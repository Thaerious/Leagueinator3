using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Controls;
using Leagueinator.GUI.Forms.Main;
using Leagueinator.GUI.src.Controllers;
using Leagueinator.GUI.Utility;
using Leagueinator.GUI.Utility.Extensions;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace Leagueinator.GUI {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            // UI thread exceptions
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            // Non-UI thread exceptions
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            // Setup Event Handlers
            this.Dispatcher.InvokeAsync(() => {
                var main = (MainWindow)this.MainWindow;

                main.Loaded += (object s, RoutedEventArgs e) => {
                    MainController mainController = new(main);
                    FocusController focusController = new(mainController);

                    // Initialize controller listeners for handling UI generated events
                    main.OnDragEnd         += mainController.DragEndHnd;
                    main.OnNamedEvent      += mainController.NamedEventHnd;
                    main.OnNamedEvent      += focusController.NamedEventHnd;

                    // Initialize UI listeners for handling controller generated events
                    mainController.OnUpdateRound    += this.UpdateRoundHnd;
                    mainController.OnSetRoundCount  += this.SetRoundCountHnd;
                    mainController.OnSetTitle       += this.SetTitleHnd;
                    focusController.OnFocusGranted  += this.GrantFocus;
                    focusController.OnFocusRevoked  += this.RevokeFocus;

                    main.Ready();
                };
            });
        }

        private void GrantFocus(object sender, FocusController.FocusArgs args) {
            TeamCard? card = this.MainWindow
                                .GetDescendantsOfType<TeamCard>()
                                .Where(card => card.MatchCard.Lane.Equals(args.TeamId.Lane))
                                .FirstOrDefault(card => card.TeamIndex.Equals(args.TeamId.TeamIndex));

            if (card is not null) card.Background = Colors.TeamPanelFocused;
        }

        private void RevokeFocus(object sender, FocusController.FocusArgs args) {
            TeamCard? card = this.MainWindow
                                .GetDescendantsOfType<TeamCard>()
                                .Where(card => card.MatchCard.Lane.Equals(args.TeamId.Lane))
                                .FirstOrDefault(card => card.TeamIndex.Equals(args.TeamId.TeamIndex));

            if (card is not null) card.Background = Colors.TeamPanelDefault;

        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            Debug.WriteLine($"UI Thread Exception:\n{e.Exception}");
            MessageBox.Show($"UI Thread Exception:\n\n{e.Exception}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            if (e.ExceptionObject is Exception ex) {
                Debug.WriteLine($"Background Thread Exception:\n{ex}");
                MessageBox.Show($"Background Thread Exception:\n\n{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) {
            Debug.WriteLine($"Unobserved Task Exception:\n{e.Exception}");
            MessageBox.Show($"Unobserved Task Exception:\n\n{e.Exception}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.SetObserved();
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
            main.RoundButtonStackPanel.Children.Clear();
            for (int i = 0; i < count; i++) {
                main.AddRoundButton();
            }
        }

        private void UpdateRoundHnd(object sender, RoundEventData args) {
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
