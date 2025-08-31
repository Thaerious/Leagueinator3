using Leagueinator.GUI.Controllers.Modules.AssignLanes;
using Leagueinator.GUI.Controllers.Modules.ELO;
using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Enums;
using Leagueinator.Utility.Extensions;
using System.Windows;

namespace Leagueinator.GUI.Controllers.Modules.Motley {

    public class MotleyModule : BaseModule {

        public static readonly Dictionary<GameResult, int> ResultValue = new() {
            { GameResult.Vacant, 0 },
            { GameResult.Loss, 1 },
            { GameResult.Tie, 2 },
            { GameResult.Win, 3 }
        };

        public override void LoadModule(Window window, MainController mainController) {
            base.LoadModule(window, mainController);
            this.MainWindow.MainMenu.AddMenuItem(["Action", "Generate Next Round"], this.NewRound);
        }

        public override void UnloadModule() {
            this.MainWindow.MainMenu.RemoveMenuItem(["Action", "Generate Next Round"]);
            NamedEvent.RemoveHandler(this);
        }

        private void NewRound(object sender, RoutedEventArgs e) {
            RoundData current = this.MainController.RoundData;
            BinSeedPlayers assignPlayers = new(current);
            RoundData playersAssigned = assignPlayers.NewRound();
            playersAssigned.Fill();
            RoundData lanesAssigned = LaneAssigner.NewRound(playersAssigned);
            this.MainController.EventData.AddRound(lanesAssigned);
            this.MainController.DispatchModel();
        }
    }
}
