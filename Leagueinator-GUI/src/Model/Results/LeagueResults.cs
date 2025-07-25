﻿
namespace Leagueinator.GUI.Model.Results {


    public class LeagueResults : List<EventResults> {
        
        public LeagueResults(LeagueData leagueData) : base() {
            foreach (EventData eventData in leagueData) {
                this.Add(new EventResults(eventData));
            }
        }
    }
}
