using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Controls.MatchCards;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.ViewModel;
using System.Diagnostics;
using System.Windows.Controls;
using Utility.Extensions;

namespace Leagueinator.GUI.Controls.MatchPanel {
    /// <summary>
    /// Control logic for MatchPanel.xaml
    /// </summary>
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
        internal void DoTieBreakerUpdated(int lane, int teamIndex, int position, string name) {
            MatchCard matchCard = this.GetMatchCard(lane);
            TeamCard teamCard = matchCard.GetTeamCard(teamIndex);
            teamCard[position] = name;
        }

        [NamedEventHandler(EventName.EventSelected)]
        [NamedEventHandler(EventName.RoundDeleted)]
        [NamedEventHandler(EventName.RoundAdded)]
        [NamedEventHandler(EventName.RoundChanged)]
        [NamedEventHandler(EventName.SetModel)]
        internal void DoSetModel(List<MatchRecord> matchRecords, List<PlayerRecord> playerRecords) {            
            this.PauseEvents();
            this.DoPopulateMatchCards(matchRecords, playerRecords);
            this.ResumeEvents();
        }
    }
}
