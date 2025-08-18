
namespace Leagueinator.GUI.Model {
    public static class Linq {
        /// <summary>
        /// Retrieve all matches.
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public static IEnumerable<MatchData> Matches(this EventData eventData, bool notEmpty = true) {
            if (notEmpty) {
                return eventData.Rounds.SelectMany(r => r.Matches).Where(m => m.HasPlayers());
            }
            else {
                return eventData.Rounds.SelectMany(r => r.Matches);
            }
        }

        /// <summary>
        /// Retrieve all matches.
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public static IEnumerable<MatchData> Matches(this LeagueData leagueData, bool notEmpty = true) {
            return leagueData.Events.SelectMany(e => e.Matches(notEmpty));
        }
    }
}
