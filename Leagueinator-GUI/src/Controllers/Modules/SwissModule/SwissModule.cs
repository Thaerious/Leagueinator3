using Algorithms;
using Leagueinator.GUI.Controllers.Modules.AssignLanes;
using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Model;
using Leagueinator.Utility.Extensions;
using System.Windows;

namespace Leagueinator.GUI.Controllers.Modules.Swiss {
    public class SwissModule : BaseModule {

        public override void LoadModule(Window window, MainController mainController) {
            base.LoadModule(window, mainController);
            this.MainWindow.MainMenu.AddMenuItem(["Action", "Generate Next Round"], this.GenerateRound);
            NamedEvent.RegisterHandler(this);
        }

        public override void UnloadModule() {
            this.MainWindow.MainMenu.RemoveMenuItem(["Action", "Generate Next Round"]);
            NamedEvent.RemoveHandler(this);
        }

        private void GenerateRound(object sender, RoutedEventArgs args) {
            this.DispatchEvent(EventName.GenerateRound);
        }

        [NamedEventHandler(EventName.GenerateRound)]
        internal void DoGenerateRound() {
            RoundData? newRound = default;

            try {
                this.MainWindow.ClearFocus();
                newRound = RoundFactory.Generate(this.MainController.EventData);
                newRound = LaneAssigner.NewRound(newRound);
                this.MainController.AddRound(newRound);
            }
            catch (UnsolvedException ex) {
                this.DispatchEvent(EventName.Notification, new() {
                    ["alertLevel"] = AlertLevel.Inform,
                    ["message"] = "Could not assign unique lanes to all teams."
                });
            }
            finally {
                if (newRound != default) this.MainController.AddRound(newRound);
            }
        }
    }
}
