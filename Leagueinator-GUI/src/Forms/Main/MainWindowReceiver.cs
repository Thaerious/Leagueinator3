using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Controls;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Utility.Extensions;

namespace Leagueinator.GUI.Forms.Main {
    public class MainWindowReceiver : NamedEventReceiver {

        private MainWindow MainWindow { get; set; }

        public MainWindowReceiver(MainWindow mainWindow) : base() {
            this.MainWindow = mainWindow;
        }

        internal void DoGrantFocus(int lane, int teamIndex) {
            TeamCard? card = this.MainWindow
                                 .GetDescendantsOfType<TeamCard>()
                                 .Where(card => card.MatchCard.Lane.Equals(lane))
                                 .FirstOrDefault(card => card.TeamIndex.Equals(teamIndex));

            if (card is not null) card.Background = Colors.TeamPanelFocused;
        }

        internal void DoRevokeFocus(int lane, int teamIndex) {
            TeamCard? card = this.MainWindow
                                .GetDescendantsOfType<TeamCard>()
                                .Where(card => card.MatchCard.Lane.Equals(lane))
                                .FirstOrDefault(card => card.TeamIndex.Equals(teamIndex));

            if (card is not null) card.Background = Colors.TeamPanelDefault;
        }

        internal void DoSetTitle(string title, bool saved) {

            if (saved) {
                this.MainWindow.Title = $"{title} [✔]";
            }
            else {
                this.MainWindow.Title = $"{title} [✘]";
            }
        }

        internal void DoUpdateRoundCount(int count) {
            this.MainWindow.RoundButtonStackPanel.Children.Clear();
            for (int i = 0; i < count; i++) {
                this.MainWindow.AddRoundButton();
            }
        }

        internal void DoRoundUpdated(int roundIndex, RoundData roundData) {
            this.MainWindow.HighLightRound(roundIndex);
            this.MainWindow.PopulateMatchCards(roundData);
        }

        internal void DoRoundRemoved(int roundIndex) {
            this.MainWindow.RemoveRound(roundIndex);
        }

        internal void DoRoundAdded(int roundIndex, RoundData roundData) {
            this.MainWindow.AddRoundButton();
        }

        internal void DoMatchRemoved(int lane) {
            this.MainWindow.RemoveMatch(lane);
        }
    }
}
