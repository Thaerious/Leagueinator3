using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Enums;
using System.Collections;
using System.Diagnostics;
using Utility;

namespace Leagueinator.GUI.Controllers.Modules.ELO {

    /// <summary>
    /// ELOEngine rating calculator for players in the league.
    /// Stores and updates ELOEngine ratings for players based on match outcomes.
    /// </summary>
    public class ELOEngine : IEnumerable<KeyValuePair<string, int>>, IEnumerable {

        // Backing store for player ELOEngine ratings.
        // DefaultDictionary ensures players not seen yet start at 2000 rating.
        private readonly DefaultDictionary<string, int> _elo = new(2000);

        /// <summary>
        /// Indexer: get the ELOEngine rating of a player by name.
        /// </summary>
        public int this[string name] => this._elo[name];

        /// <summary>
        /// Checks if a given player has an ELOEngine entry.
        /// </summary>
        public bool ContainsKey(string name) => this._elo.ContainsKey(name);

        /// <summary>
        /// Returns all player names currently tracked.
        /// </summary>
        public IEnumerable<string> Keys => this._elo.Keys;

        /// <summary>
        /// Number of players with ratings stored.
        /// </summary>
        public int Count => this._elo.Count;

        /// <summary>
        /// Constructs an ELOEngine rating system from league data.
        /// Iterates through matches in chronological order and updates ratings.
        /// </summary>
        public ELOEngine(LeagueData leagueData) {
            // Sort events chronologically by date.
            List<EventData> events = [.. leagueData.Events];
            events.Sort((ed1, ed2) => ed1.Date.CompareTo(ed2.Date));

            // Process every match in the league to update ratings.
            foreach (MatchData matchData in leagueData.Matches()) {
                UpdateElo(matchData);
            }
        }

        /// <summary>
        /// Returns the combined ELOEngine rating of a group of players.
        /// </summary>
        /// <param name="names">Collection of player names.</param>
        /// <returns>Total ELOEngine rating for all players in the group.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown if any player in <paramref name="names"/> does not have an ELOEngine entry.
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
        /// Updates ELOEngine ratings for all players in a given match.
        /// Compares each winning team against each losing team.
        /// Each player on the winning team gets points, each on the losing team loses points.
        /// </summary>
        private void UpdateElo(MatchData matchData) {
            DefaultDictionary<string, int> delta = new(0);

            var teams = matchData.Teams.Where(t => t.HasPlayers()).ToList();

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

                    // Update ELOEngine for every player-vs-player combination.
                    foreach (string winningPlayer in winner.AllNames()) {
                        foreach (string losingPlayer in loser.AllNames()) {
                            int deltaELO = this.DeltaElo(winningPlayer, losingPlayer) / teamData2.CountPlayers();
                            delta[winningPlayer] += deltaELO ;
                            delta[losingPlayer] -= deltaELO;
                        }
                    }
                }
            }

            foreach (string name in delta.Keys) {
                this._elo[name] += delta[name];
            }
        }

        /// <summary>
        /// Computes the ELOEngine rating change when one player beats another.
        /// </summary>
        private int DeltaElo(string winner, string loser) {
            int winnerELO = this._elo[winner];
            int loserELO = this._elo[loser];

            // Constant factor determining sensitivity of rating changes.
            const int K = 32;

            // Expected win probability for the winner.
            double expectedWin = 1.0 / (1.0 + Math.Pow(10, (loserELO - winnerELO) / 400.0));

            // Rating gain = K * (1 - expected score)
            return (int)(K * (1 - expectedWin));
        }

        public IEnumerator<KeyValuePair<string, int>> GetEnumerator() => _elo.GetEnumerator();  // generic

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_elo).GetEnumerator();  // non-generic
    }
}
