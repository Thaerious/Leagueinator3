using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Controls.MatchCards;
using Leagueinator.GUI.Model.ViewModel;
using System.Windows.Controls;

namespace Leagueinator.GUI.Controls.MatchPanel {
    /// <summary>
    /// Control logic for MatchPanel.xaml
    /// </summary>
    [NamedEventContext("UIContext")]
    public partial class MatchPanel : UserControl {
        [NamedEventHandler(EventName.MatchRemoved)]
        internal void DoMatchRemoved(int lane) {
            this.RemoveMatch(lane);
        }

        [NamedEventHandler(EventName.BowlsUpdated)]
        internal void DoBowlsUpdated(int lane, int[] bowls) {
            MatchCard matchCard = this.GetMatchCard(lane);
            matchCard.SetBowls(bowls);
        }

        [NamedEventHandler(EventName.EndsUpdated)]
        internal void DoEndsUpdated(int lane, int ends) {
            MatchCard matchCard = this.GetMatchCard(lane);
            matchCard.SetEnds(ends);
        }

        [NamedEventHandler(EventName.TieBreakerUpdated)]
        internal void DoTieBreakerUpdated(int lane, int teamIndex) {
            MatchCard matchCard = this.GetMatchCard(lane);
            matchCard.SetTieBreaker(teamIndex);
        }


        [NamedEventHandler(EventName.NameUpdated)]
        internal void DoNameUpdated(int lane, int teamIndex, int position, string name, List<string> nameAlerts, HashSet<int> laneAlerts) {
            MatchCard matchCard = this.GetMatchCard(lane);
            TeamCard teamCard = matchCard.GetTeamCard(teamIndex);
            this.HighlightNames(nameAlerts);
            this.HighlightLanes(laneAlerts);
            teamCard[position] = name;
        }

        [NamedEventHandler(EventName.EventSelected)]
        [NamedEventHandler(EventName.RoundDeleted)]
        [NamedEventHandler(EventName.RoundAdded)]
        [NamedEventHandler(EventName.RoundChanged)]
        [NamedEventHandler(EventName.SetModel)]
        async internal void DoSetModel(List<MatchRecord> matchRecords, List<PlayerRecord> playerRecords, List<string> nameAlerts, HashSet<int> laneAlerts) {            
            this.PauseEvents();
            await this.DoPopulateMatchCards(matchRecords, playerRecords, nameAlerts, laneAlerts);
            this.ResumeEvents();
        }
    }
}
