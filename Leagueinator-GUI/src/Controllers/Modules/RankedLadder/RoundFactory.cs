using Algorithms.Mapper;
using Leagueinator.GUI.Model;
using System.Diagnostics;
using Utility;
using Utility.Collections;
using Utility.Extensions;

namespace Leagueinator.GUI.Controllers.Modules.RankedLadder {
    /// <summary>
    /// Provides logic to assign lanes to matches in a round, ensuring that teams are not repeatedly assigned to the same lanes across _rounds.
    /// </summary>
    /// 
    public static class RoundFactory {
        public static RoundData Generate(EventData eventData) {
            throw new NotImplementedException();
            //List<Players> keys = scoringModule.EventRankingByTeam(eventData);                     
            
            //Debug.WriteLine($"Generate map with keys: {keys.JoinString(prefix:"[", suffix:"]")}");
            //MultiMap<Players, Players> blacklist = eventData.PreviousOpponents();
            //DFSListPairMapper<Players> mapGenerator = new();
            //var matchMap = mapGenerator.GenerateMap(keys, blacklist);
            //var newRound = new RoundData(eventData);

            //foreach (var (key, value) in matchMap) {
            //    MatchData matchData = new(newRound) {
            //        MatchFormat = eventData.DefaultMatchFormat,
            //        Ends = eventData.DefaultEnds
            //    };
            //    matchData.Teams[0].CopyFrom(key);
            //    matchData.Teams[1].CopyFrom(value);
            //    newRound.AddMatch(matchData);
            //}

            //newRound.Fill();
            //return newRound;
        }
    }
}
