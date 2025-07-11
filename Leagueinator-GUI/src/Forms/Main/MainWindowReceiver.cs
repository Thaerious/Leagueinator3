using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Controls;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Utility.Extensions;
using System.Windows;

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
        internal void DoRoundUpdated(int roundIndex, RoundRecordList roundRecords) {
            this.MainWindow.HighLightRound(roundIndex);
            this.MainWindow.PopulateMatchCards(roundRecords);
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

        [NamedEventHandler(EventName.BowlsUpdated)]
        internal void DoBowlsUpdated(int lane, int teamIndex, int bowls) {
            MatchCard matchCard = this.MainWindow.GetMatchCard(lane);
            TeamCard teamCard = matchCard.GetTeamCard(teamIndex) ?? throw new KeyNotFoundException($"TeamIndex {teamIndex} not found.");
            teamCard.BowlsPanel.SetBowls(bowls);
        }

        [NamedEventHandler(EventName.EndsUpdated)]
        internal void DoEndsUpdated(int lane, int ends) {
            MatchCard matchCard = this.MainWindow.GetMatchCard(lane);
            matchCard.SetEnds(ends);
        }

        [NamedEventHandler(EventName.TieBreakerUpdated)]
        internal void DoTieBreakerUpdated(int lane, int tieBreaker) {
            MatchCard matchCard = this.MainWindow.GetMatchCard(lane);
            matchCard.SetTieBreaker(tieBreaker);
        }


        [NamedEventHandler(EventName.NameUpdated)]
        internal void DoTieBreakerUpdated(int lane, int teamIndex, int position, string name) {
            MatchCard matchCard = this.MainWindow.GetMatchCard(lane);
            TeamCard teamCard = matchCard.GetTeamCard(teamIndex) ?? throw new KeyNotFoundException();
            teamCard[position] = name;
        }

        [NamedEventHandler(EventName.Notification)]
        internal void DoNotification(string message) {
            MessageBox.Show(message, "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
