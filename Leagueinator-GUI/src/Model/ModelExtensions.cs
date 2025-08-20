
namespace Leagueinator.GUI.Model {
    public static class ModelExtensions {

        public static IEnumerable<string> AllNames(this IEnumerable<TeamData> teams) {
            return teams.SelectMany(t => t.Players);
        }

        public static IEnumerable<TeamData> GetOpposition(this TeamData teamData) {
            MatchData match = teamData.Parent;
            return match.Teams.Where(that => that != teamData);
        }

        public static IEnumerable<TeamData> Opponents(this IEnumerable<TeamData> teams) {
            return teams.SelectMany(t => t.GetOpposition());
        }
    }
}
