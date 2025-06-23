
namespace Leagueinator.GUI.Model.Results {
    public class RoundDataCollection : List<RoundData> {


        /// <summary>
        /// Returns a list of all matches that the given team has played in.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public List<MatchData> GetMatchesForTeam(string[] team) {
            List<MatchData> matches = [];

            foreach (RoundData round in this) {
                foreach (MatchData match in round) {
                    foreach (string[] matchTeam in match.Teams) {
                        if (matchTeam.OrderBy(x => x).SequenceEqual(team)) {
                            matches.Add(match);
                        }
                    }
                }
            }
            return matches;
        }

        /// <summary>
        /// Returns a list of all previous prevOpponent for the given team.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public List<string[]> PreviousOpponents(string[] team) {
            List<string[]> opponents = [];

            foreach (MatchData match in this.GetMatchesForTeam(team)) {
                foreach (string[] opposingTeam in match.Teams) {
                    if (opposingTeam.Intersect(opposingTeam).Any()) continue; // Skip our own team
                    var unused = opponents.Append(opposingTeam);
                }
            }

            return opponents;
        }

        /// <summary>
        /// True if any player from team has played against any player from pollOpponent.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public bool HasPlayed(string[] team, string[] pollOpponent) {
            foreach (string[] prevOpponent in this.PreviousOpponents(team)) {
                if (pollOpponent.Intersect(prevOpponent).Any()) {
                    return true;
                }
            }
            return false;
        }
    }
}
