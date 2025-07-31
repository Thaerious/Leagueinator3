using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Results.BowlsPlus;
using Utility;
using Utility.Extensions;

namespace Leagueinator.GUI.Controllers.Modules.Motley {
    public static class ELO {
        public static Dictionary<string, int> CalculateELO(LeagueData league) {
            // Returns a map from user-name -> deltaELO-score
            DefaultDictionary<string, int> elo = new((key)=>2000);

            // Sort the event in chronological order
            league.Sort((ed1, ed2) => ed1.Date.CompareTo(ed2.Date));

            foreach (EventData eventData in league) {
                // Convert the event data to an event result
                EventResults eventResults = new(eventData);

                // Cycle through each match result
                foreach (RoundResults roundResults in eventResults.ByRound) {
                    foreach (MatchResult matchResult in roundResults.ByMatch) {
                        if (matchResult.Count == 0) continue;
                        UpdateElo(matchResult, elo);
                    }
                }
            }

            return elo;
        }

        private static void UpdateElo(MatchResult matchResult, DefaultDictionary<string, int> elo) {
            DefaultDictionary<string, int> deltaELO = new((key) => 0);

            List<SingleResult> results = [.. matchResult.Where(s => s.Players.Count > 0)];

            while (results.Count > 0) {
                SingleResult s1 = results.Pop();
                foreach (SingleResult s2 in results) {
                    CalculateElo(s1, s2, deltaELO);
                }
            }

            foreach (var kvp in deltaELO) {
                var name = kvp.Key;
                var eloChange = kvp.Value / (matchResult.Count - 1);    
                elo[name] = elo[name] + eloChange;
            }
        }

        private static void CalculateElo(SingleResult s1, SingleResult s2, DefaultDictionary<string, int> eloDiff) {
            Players winners, losers;

            if (s1.CompareTo(s2) == 0) {
                return;
            }
            if (s1.CompareTo(s2) > 0) {
                winners = s1.Players;
                losers = s2.Players;
            }
            else {
                winners = s2.Players;
                losers = s1.Players;
            }

            int winnerELO = winners.Where(name => !string.IsNullOrEmpty(name)).Select(name => eloDiff[name]).Sum() / winners.Count;
            int loserELO = losers.Where(name => !string.IsNullOrEmpty(name)).Select(name => eloDiff[name]).Sum() / winners.Count;

            const int K = 32;

            double expectedWin = 1.0 / (1.0 + Math.Pow(10, (loserELO - winnerELO) / 400.0));
            double eloChange = K * (1 - expectedWin);

            // Apply to each player evenly
            foreach (var name in winners) {
                if (string.IsNullOrEmpty(name)) continue;
                eloDiff[name] += (int)Math.Ceiling(eloChange);
            }
            foreach (var name in losers) {
                if (string.IsNullOrEmpty(name)) continue;
                eloDiff[name] -= (int)Math.Floor(eloChange);
            }
        }
    }
}
