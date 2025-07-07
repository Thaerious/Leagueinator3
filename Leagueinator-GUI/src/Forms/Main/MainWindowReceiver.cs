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

        [NamedEventHandler(EventName.FocusGranted)]
        internal void DoFocusGranted(int lane, int teamIndex) {
            TeamCard? card = this.MainWindow
                                 .GetDescendantsOfType<TeamCard>()
                                 .Where(card => card.MatchCard.Lane.Equals(lane))
                                 .FirstOrDefault(card => card.TeamIndex.Equals(teamIndex));

            if (card is not null) card.Background = Colors.TeamPanelFocused;
        }

        [NamedEventHandler(EventName.FocusRevoked)]
        internal void DoFocusRevoked(int lane, int teamIndex) {
            TeamCard? card = this.MainWindow
                                .GetDescendantsOfType<TeamCard>()
                                .Where(card => card.MatchCard.Lane.Equals(lane))
                                .FirstOrDefault(card => card.TeamIndex.Equals(teamIndex));

            if (card is not null) card.Background = Colors.TeamPanelDefault;
        }

        [NamedEventHandler(EventName.SetTitle)]
        internal void DoSetTitle(string title, bool saved) {

            if (saved) {
                this.MainWindow.Title = $"{title} [✔]";
            }
            else {
                this.MainWindow.Title = $"{title} [✘]";
            }
        }

        [NamedEventHandler(EventName.UpdateRoundCount)]
        internal void DoUpdateRoundCount(int count) {
            this.MainWindow.RoundButtonStackPanel.Children.Clear();
            for (int i = 0; i < count; i++) {
                this.MainWindow.AddRoundButton();
            }
        }

        [NamedEventHandler(EventName.RoundUpdated)]
        internal void DoRoundUpdated(int roundIndex, RoundData roundData) {
            this.MainWindow.HighLightRound(roundIndex);
            this.MainWindow.PopulateMatchCards(roundData);
        }

        [NamedEventHandler(EventName.RoundRemoved)]
        internal void DoRoundRemoved(int roundIndex) {
            this.MainWindow.RemoveRound(roundIndex);
        }

        [NamedEventHandler(EventName.RoundAdded)]
        internal void DoRoundAdded() {
            this.MainWindow.AddRoundButton();
        }

        [NamedEventHandler(EventName.MatchRemoved)]
        internal void DoMatchRemoved(int lane) {
            this.MainWindow.RemoveMatch(lane);
        }
    }
}
