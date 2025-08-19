
namespace Leagueinator.GUI.Model {
    public static class ModelExtensions {

        public static IEnumerable<string> AllNames(this IEnumerable<TeamData> teams) {
            return teams.SelectMany(t => t.AllNames());
        }

    }
}
