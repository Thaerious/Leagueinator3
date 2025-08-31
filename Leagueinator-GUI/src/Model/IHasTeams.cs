namespace Leagueinator.GUI.Model {
    public interface IHasTeams {
        IEnumerable<TeamData> AllTeams();

        IEnumerable<string> AllNames() => this.AllTeams().SelectMany(t => t.ToPlayers());
    }

    public static class HasTeamExtensons {
        public static IEnumerable<string> AllNames(this IHasTeams hasTeams) => hasTeams.AllNames();
    }
}
