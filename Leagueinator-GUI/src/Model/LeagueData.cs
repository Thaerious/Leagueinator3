namespace Leagueinator.GUI.Model {
    public class LeagueData : List<EventData>{

        private int NextUID = 1;
        public int GetNextUID() {
            return this.NextUID++;
        }

    }
}
