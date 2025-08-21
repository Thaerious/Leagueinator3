using Algorithms.Mapper;
using Leagueinator.GUI.Model;
using System.Diagnostics;
using Utility;
using Utility.Extensions;

namespace Leagueinator.GUI.Controllers.Modules.AssignLanes {
    /// <summary>
    /// Provides logic to assign lanes to matches in a round, ensuring that teams are not repeatedly assigned to the same lanes across _rounds.
    /// </summary>
    /// 
    public static class LaneAssigner {
        public static RoundData NewRound(RoundData roundData) {
            MultiMap<MatchData, int> blacklist = [];
            EventData eventData = roundData.Parent;
            var previousLanes = eventData.PreviousLanes(except: roundData);

            foreach (MatchData matchData in roundData.Matches) {
                foreach (TeamData teamData in matchData.Teams) {
                    blacklist[matchData].AddRange(previousLanes[teamData.Players]);
                }
            };

            foreach (MatchData matchData in blacklist.Keys) {
                var values = blacklist[matchData];
            }

            ConstrainedDFSMapper<MatchData, int> mapGenerator = new();
            var lanesAvailable = Enumerable.Range(0, eventData.LaneCount);
            var laneMap = mapGenerator.GenerateMap(roundData.Matches, lanesAvailable, blacklist);

            var newRound = new RoundData(eventData).Fill();

            foreach (MatchData matchData in laneMap.Keys) {
                var newMatch = matchData.Copy(newRound);
                int newLane = laneMap[matchData];
                newRound.InsertMatch(newLane, newMatch);
            }

            return newRound;
        }
    }
}
