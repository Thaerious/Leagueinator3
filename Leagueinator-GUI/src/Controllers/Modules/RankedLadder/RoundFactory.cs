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
    public static class RoundFactory {
        public static RoundData Generate(EventData eventData) {
            MultiMap<Players, Players> blacklist = eventData.PreviousOpponents();

            foreach (var (key, val) in blacklist)
                Debug.WriteLine($"{key} → {val.JoinString()}");

            DFSListPairMapper<Players> mapGenerator = new();
            var matchMap = mapGenerator.GenerateMap(blacklist.Keys, blacklist);
            var newRound = new RoundData(eventData);

            foreach (var (key, value) in matchMap) {
                MatchData matchData = new(newRound) {
                    MatchFormat = eventData.DefaultMatchFormat,
                    Ends = eventData.DefaultEnds
                };
                matchData.Teams[0].CopyFrom(key);
                matchData.Teams[1].CopyFrom(value);
                newRound.AddMatch(matchData);
            }

            newRound.Fill();
            return newRound;
        }
    }
}
