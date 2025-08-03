using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Controls.MatchCards;
using System.Windows.Controls;

namespace Leagueinator.GUI.Controls.MatchPanel {
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
    }
}
