using Algorithms;
using Algorithms.PairMatching;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Enums;
using System.Diagnostics;

namespace Leagueinator.GUI.Controllers.Modules.Motley {
    public class AssignPlayers {
        private EventData EventData;

        public LeagueData LeagueData { get; }
        public RoundData RoundData { get; }

        Dictionary<string, int> ELO = [];

        public AssignPlayers(LeagueData leagueData, EventData eventData, RoundData roundData) {
            this.LeagueData = leagueData;
            this.EventData = eventData;
            this.RoundData = roundData;
        }

        public RoundData Run() {
            // The new round that will be returned
            RoundData roundData = new(this.EventData);

            // Create a list of thisRoundPlayers from the ELO rankings
            this.ELO = Motley.ELO.CalculateELO(this.LeagueData);
            List<string> thisRoundPlayers = [.. RoundData.Records().Select(r => r.Name)];

            // Keep only players from the source round (ELO provides players from the whole league).
            foreach (var name in this.ELO.Keys.ToList()) {
                if (!thisRoundPlayers.Contains(name)) {
                    this.ELO.Remove(name);
                }
            }

            // Add new players from this round
            foreach (var player in thisRoundPlayers) {
                if (!this.ELO.ContainsKey(player)) {
                    this.ELO[player] = 2000;
                }
            }

            // Extra players
            List<string> setAside = [];

            // Make the ELO list even
            if (ELO.Count % 2 != 0) {
                setAside = this.SetAside(3);
                foreach (string name in setAside) this.ELO.Remove(name);

                // Assign those players to a 4321 match
                roundData.AddMatch(new MatchData(roundData) {
                    MatchFormat = MatchFormat.A4321,
                    Ends = this.EventData.DefaultEnds
                });
            }

            this.AssignTeams(roundData);
            var matchesAssigned = this.AssignMatches(roundData);
            return matchesAssigned;
            
        }

        private RoundData AssignTeams(RoundData roundData) {
            // Build a graph, players are nodes, connected to players they can play with.
            // The edges are weighted with the elo differece between the two players.
            Graph<string> graph = new();
            GreedyAugmenting<string> pairingAlgorithm = new(graph);

            foreach (var forPlayer in this.ELO) {
                List<string> previousPartners = this.EventData.AllTeams()
                                                    .Where(t => t.Names.Contains(forPlayer.Key))
                                                    .SelectMany(t => t.Names)
                                                    .Where(n => n != forPlayer.Key)
                                                    .ToList();

                // Add edges to graph between all players haven't been partners this round
                foreach (var otherPlayer in this.ELO) {
                    if (forPlayer.Key == otherPlayer.Key) continue;
                    if (!previousPartners.Contains(otherPlayer.Key)) {
                        int diff = Math.Abs(forPlayer.Value - otherPlayer.Value);
                        graph.AddEdge(forPlayer.Key, otherPlayer.Key, diff * diff);
                    }
                }
            }

            Debug.WriteLine(graph.ToCSV());

            // Run the pairing algorithm to pair players up.
            Solution<string>? best = pairingAlgorithm.Run(50, 50);
            if (best == null) throw new Exception("No pairings generated");

            Debug.WriteLine($"Fitness = {best.Fitness}");

            // Add matches to the round for each pairing (1 match for 2 pairs).
            int team = 0; 
            for (int i = 0; i < best!.Count; i++) {
                MatchData match = new(roundData) {
                    MatchFormat = MatchFormat.VS2,
                    Ends = this.EventData.DefaultEnds
                };

                Debug.WriteLine($"{best[i].Item1}, {best[i].Item2}, {graph.GetEdge(best[i].Item1, best[i].Item2).Weight}");
                match.Teams[team].CopyFrom([best[i].Item1, best[i].Item2]);
                roundData.AddMatch(match);
                team = team == 0 ? 1 : 0; // team flips between 0 and 1
            }

            return roundData;
        }

        private RoundData AssignMatches(RoundData roundData) {
            Dictionary<TeamData, int> teamELO = [];

            var teams = roundData.Matches.SelectMany(match => match.Teams);

            foreach (TeamData team in teams) {
                if (team.IsEmpty()) continue;
                int elo = 0;
                foreach (string name in team) {
                    elo += this.ELO[name];
                }
                elo /= team.CountPlayers();
                teamELO[team] = elo;
            }

            var orderdTeams = teamELO
                             .OrderBy(kvp => teamELO[kvp.Key])
                             .Select(kvp => kvp.Key)
                             .ToList();

            RoundData newRound = new(this.EventData);
            for (int i = 0; i < orderdTeams.Count; i += 2) {
                MatchData matchData = new(newRound) {
                    MatchFormat = MatchFormat.VS2,
                    Ends = this.EventData.DefaultEnds
                };

                matchData.Teams[0].CopyFrom(orderdTeams[i]);
                matchData.Teams[1].CopyFrom(orderdTeams[i + 1]);
                newRound.AddMatch(matchData);
            }

            return newRound;
        }

        /// <summary>
        /// Set aside 3 players to play 4-3-2-1
        /// </summary>
        private List<string> SetAside(int count) {
            Dictionary<string, int> localELO = new(this.ELO);

            this.EventData
                .Records()
                .Where(r => r.MatchFormat == MatchFormat.A4321)
                .ToList().ForEach(r => {
                    localELO.Remove(r.Name);
                });

            var sortedList = localELO.OrderBy(kv => kv.Value).Select(kv => kv.Key).ToList();
            sortedList.RemoveRange(0, sortedList.Count - count);
            return sortedList;
        }
    }
}
