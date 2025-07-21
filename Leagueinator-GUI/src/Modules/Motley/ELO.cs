using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Results;

namespace Leagueinator.GUI.Modules.Motley {
    public class ELO {
        public static Dictionary<string, int> CalculateELO(LeagueData league) {
            Dictionary<string, int> elo = [];
            league.Sort((ed1, ed2) => ed1.Date.CompareTo(ed2.Date));

            foreach (EventData eventData in league) {
                EventResults eventResults = new(eventData);

                foreach (RoundResults roundResults in eventResults.ByRound) {
                    foreach (MatchResult matchResult in roundResults.ByMatch) {
                        AddPlayersToElo(matchResult, elo);
                        UpdateElo(matchResult, elo);
                    }
                }
            }

            return elo;
        }

        private static void UpdateElo(MatchResult matchResult, Dictionary<string, int> elo) {
            Players winners = matchResult.GetWinners();
            Players losers = matchResult.GetLosers();

            int winnerELO = winners.Select(name => elo[name]).Sum() / winners.Count;
            int loserELO = losers.Select(name => elo[name]).Sum() / winners.Count;

            const int K = 32;

            double expectedWin = 1.0 / (1.0 + Math.Pow(10, (loserELO - winnerELO) / 400.0));
            int eloChange = (int)Math.Round(K * (1 - expectedWin));

            // Apply to each player evenly
            foreach (var name in winners) {
                elo[name] += eloChange;
            }
            foreach (var name in losers) {
                elo[name] -= eloChange;
            }
        }

        private static void AddPlayersToElo(MatchResult matchResult, Dictionary<string, int> elo) {
            foreach (SingleResult singleResult in matchResult) {
                foreach (string player in singleResult.Players) {
                    if (!elo.ContainsKey(player)) {
                        elo[player] = 2000;
                    }
                }
            }
        }
    }
}
