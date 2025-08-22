using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Enums;
using Utility;

namespace Leagueinator.GUI.Controllers.Modules.ELO {

    /// <summary>
    /// ELODictionary rating calculator for players in the league.
    /// Stores and updates ELODictionary ratings for players based on match outcomes.
    /// </summary>
    public class ELODictionary : Dictionary<string, int> {        
        /// <summary>
        /// Constructs an ELODictionary rating system from league data.
        /// Iterates through matches in chronological order and updates ratings.
        /// </summary>
        public ELODictionary(LeagueData leagueData) {
            foreach (string name in leagueData.AllNames()) {
                this[name] = 2000;
            }

            // Sort events chronologically by date.
            List<EventData> events = [.. leagueData.Events];
            events.Sort((ed1, ed2) => ed1.Date.CompareTo(ed2.Date));

            // Process every match in the league to update ratings.
            foreach (MatchData matchData in leagueData.Matches()) {
                UpdateElo(matchData);
            }
        }

        public ELODictionary(EventData eventData) : this(eventData.Parent) {
            var eventNames = eventData.AllNames();

            foreach (string name in this.Keys) {
                if (!eventNames.Contains(name)) this.Remove(name);
            }
        }

        public ELODictionary(RoundData roundData) : this(roundData.Parent) {
            var eventNames = roundData.AllNames();

            foreach (string name in this.Keys) {
                if (!eventNames.Contains(name)) this.Remove(name);
            }
        }

        /// <summary>
        /// Returns the combined ELODictionary rating of a group of players.
        /// </summary>
        /// <param name="names">Collection of player names.</param>
        /// <returns>Total ELODictionary rating for all players in the group.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown if any player in <paramref name="names"/> does not have an ELODictionary entry.
        /// </exception>
        public int GetELO(IEnumerable<string> names) {
            int sum = 0;
            foreach (string name in names) {
                if (string.IsNullOrEmpty(name)) continue; // Ignore empty names.
                if (!this.ContainsKey(name)) throw new KeyNotFoundException(name); // Explicitly fail if unknown player.
                sum += this[name];
            }
            return sum;
        }


        /// <summary>
        /// Updates ELODictionary ratings for all players in a given match.
        /// Compares each winning team against each losing team.
        /// Each player on the winning team gets points, each on the losing team loses points.
        /// </summary>
        private void UpdateElo(MatchData matchData) {
            DefaultDictionary<string, int> delta = new(0);

            var teams = matchData.Teams.Where(t => !t.IsEmpty()).ToList();

            for (int i = 0; i < teams.Count; i++) {
                for (int j = i + 1; j < teams.Count; j++) {
                    TeamData teamData1 = teams[i];
                    TeamData teamData2 = teams[j];

                    TeamData winner, loser;
                    if (teamData1.Result == GameResult.Win) {
                        winner = teamData1;
                        loser = teamData2;
                    }
                    else if (teamData2.Result == GameResult.Win) {
                        winner = teamData2;
                        loser = teamData1;
                    }
                    else {
                        continue;
                    }

                    // Update ELODictionary for every player-vs-player combination.
                    foreach (string winningPlayer in winner.ToPlayers()) {
                        foreach (string losingPlayer in loser.ToPlayers()) {
                            int deltaELO = this.DeltaElo(winningPlayer, losingPlayer) / teamData2.Names.Count;
                            delta[winningPlayer] += deltaELO ;
                            delta[losingPlayer] -= deltaELO;
                        }
                    }
                }
            }

            foreach (string name in delta.Keys) {
                this[name] += delta[name];
            }
        }

        /// <summary>
        /// Computes the ELODictionary rating change when one player beats another.
        /// </summary>
        private int DeltaElo(string winner, string loser) {
            int winnerELO = this[winner];
            int loserELO = this[loser];

            // Constant factor determining sensitivity of rating changes.
            const int K = 32;

            // Expected win probability for the winner.
            double expectedWin = 1.0 / (1.0 + Math.Pow(10, (loserELO - winnerELO) / 400.0));

            // Rating gain = K * (1 - expected score)
            return (int)(K * (1 - expectedWin));
        }
    }
}
