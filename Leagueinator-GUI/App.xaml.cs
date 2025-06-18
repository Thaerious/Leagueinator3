using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Controls;
using Leagueinator.GUI.Forms.Main;
using Leagueinator.GUI.Utility;
using System.Diagnostics;
using System.Windows;

namespace Leagueinator.GUI {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            Debug.WriteLine("Application Startup");

            this.Dispatcher.InvokeAsync((Action)(() => {
                var main = (MainWindow)this.MainWindow;

                main.Loaded += (object s, global::System.Windows.RoutedEventArgs e) => {
                    Debug.WriteLine("MainWindow Loaded");
                    MainController mainController = new(main);

                    // Initialize controller listeners for handling UI generated events
                    main.AddHandler(global::Leagueinator.GUI.Controls.MatchCard.MatchCardUpdateEvent, (global::System.Delegate)new global::System.Windows.RoutedEventHandler(mainController.MatchCardUpdateHnd));
                    main.OnRoundData += mainController.RoundDataHnd;
                    main.OnFileEvent += mainController.FileEventHnd;
                    main.OnActionEvent += mainController.ActionEventHnd;

                    // Initialize UI listeners for handling controller generated events
                    mainController.OnUpdateRound += this.UpdateRoundHnd;
                    mainController.OnSetRoundCount += this.SetRoundCountHnd;
                    mainController.OnSetTitle += this.SetTitleHnd;
                    main.Ready();
                };
            }));
        }

        private void SetTitleHnd(object sender, string filename, bool saved) {
            var main = (MainWindow)this.MainWindow;

            if (saved) {
                main.Title = $"{filename} [✔]";
            }
            else {
                main.Title = $"{filename} [✘]";
            }
        }

        private void SetRoundCountHnd(object sender, int count) {
            Logger.Log($"App.SetRoundCount: {count}");
            var main = (MainWindow)this.MainWindow;
            main.RoundButtonContainer.Children.Clear();
            for (int i = 0; i < count; i++) {
                main.AddRoundButton();
            }
        }

        private void UpdateRoundHnd(object sender, RoundEventData args) {
            Logger.Log($"App.UpdateRound: {args.Action}, {args.Index}");
            var main = (MainWindow)this.MainWindow;

            switch (args.Action) {
                case "Update":
                    if (args.RoundData == null) {
                        throw new ArgumentNullException(
                            nameof(args.RoundData),
                            "Round data cannot be null for update action."
                        );
                    }
                    main.HighLightRound(args.Index);
                    main.PopulateMatchCards(args.RoundData);
                    break;
                case "RemoveMatch":
                    main.RemoveMatch(args.Index);
                    break;
                case "RemoveRound":
                    main.RemoveRound(args.Index);
                    break;
                case "AddRound":
                    main.AddRoundButton();
                    break;
                default:
                    throw new NotSupportedException($"Action '{args.Action}' is not supported.");
            }
        }



        protected override void OnExit(ExitEventArgs e) {
            // Cleanup logic
            base.OnExit(e);
            Debug.WriteLine("Application Exit");
        }
    }

}
