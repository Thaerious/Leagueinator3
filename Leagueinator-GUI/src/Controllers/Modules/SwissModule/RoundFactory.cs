using Algorithms.Mapper;
using Leagueinator.GUI.Controllers.Modules.ScoringPlus;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Enums;
using Utility.Collections;

namespace Leagueinator.GUI.Controllers.Modules.Swiss {
    /// <summary>
    /// Provides logic to assign lanes to matches in a round, ensuring that teams are not repeatedly assigned to the same lanes across _rounds.
    /// </summary>
    /// 
    public static class RoundFactory {
        public static RoundData Generate(EventData eventData) {
            Type resultType = eventData.MatchScoring.GetResultType();
            ResultsCollection<PlusResult> results = (ResultsCollection <PlusResult>)ResultsCollection.Construct(typeof(PlusResult));
            results.AddTeams(eventData);

            List<Players> keys = [.. results.Teams];
            MultiMap<Players, Players> blacklist = eventData.PreviousOpponents();
            DFSListPairMapper<Players> mapGenerator = new();
            var matchMap = mapGenerator.GenerateMap(keys, blacklist);
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
