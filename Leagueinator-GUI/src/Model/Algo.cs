
namespace Leagueinator.GUI.Model {
    
    
    /// <summary>
    /// A collection of methods that do not manipulate a class.
    /// </summary>
    public static class Algo {

        /// <summary>
        /// Calculate the number of events since the two players last played each other.
        /// </summary>
        public static int LastPlayed(this LeagueData leagueData, string name1, string name2) {
            int count = leagueData.Events.Count;

            for (int i = count  - 1; i >= 0; i--) {
                EventData eventData = leagueData.Events[i];

                TeamData? team = eventData.Rounds.SelectMany(round => round.Matches)
                                .SelectMany(match => match.Teams)
                                .Where(t => t.Players.Contains(name1))
                                .SelectMany(t => t.GetOpposition())
                                .Where(t => t.Players.Contains(name2))
                                .FirstOrDefault();

                if (team != null) return (count - 1 - i);
            }
            return -1;
        }

        public static List<TeamData> GetOpposition(this TeamData teamData) {
            MatchData match = teamData.Parent;
            return [.. match.Teams.Where(that => that != teamData)];
        }
    }
}
