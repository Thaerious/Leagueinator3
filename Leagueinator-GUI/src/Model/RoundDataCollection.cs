
namespace Leagueinator.GUI.Model {
    public class RoundDataCollection : List<RoundData> {

        internal RoundDataCollection() { }

        /// <summary>
        /// Generate a collection of all unique teams that have played in any match across all rounds.
        /// </summary>
        public HashSet<TeamData> Teams {
            get {
                HashSet<TeamData> teams = [];

                foreach (RoundData round in this) {
                    foreach (MatchData match in round) {
                        foreach (TeamData team in match.Teams) {
                            teams.Add(team);
                        }
                    }
                }

                return teams;
            }
        }


        /// <summary>
        /// Returns a list of all matches that a team has played in.
        /// </summary>
        /// <param name="targetTeam"></param>
        /// <returns></returns>
        public List<MatchData> GetMatchesForTeam(TeamData targetTeam) {
            List<MatchData> matches = [];

            foreach (RoundData round in this) {
                foreach (MatchData match in round) {
                    foreach (TeamData consideringTeam in match.Teams) {
                        if (consideringTeam.Equals(targetTeam)) {
                            matches.Add(match);
                        }
                    }
                }
            }
            return matches;
        }

        /// <summary>
        /// Returns a list of all previous previous opponents for the Target team.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public List<TeamData> PreviousOpponents(TeamData team) {
            List<TeamData> opponents = [];

            foreach (MatchData match in this.GetMatchesForTeam(team)) {
                foreach (TeamData opposingTeam in match.Teams) {
                    if (opposingTeam.Equals(team)) continue; // Skip our own targetTeam
                    opponents.Add(opposingTeam);
                }
            }

            return opponents;
        }

        /// <summary>
        /// True if any player from targetTeam has played against any player from pollOpponent.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public bool HasPlayed(TeamData team, TeamData pollOpponent) {
            foreach (TeamData prevOpponent in this.PreviousOpponents(team)) {
                if (pollOpponent.Names.Intersect(prevOpponent.Names).Any()) {
                    return true;
                }
            }
            return false;
        }
    }
}
