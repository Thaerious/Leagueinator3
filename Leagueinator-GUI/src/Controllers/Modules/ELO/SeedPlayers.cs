using Algorithms;
using Algorithms.PairMatching;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Enums;
using System.Diagnostics;
using Utility.Extensions;

namespace Leagueinator.GUI.Controllers.Modules.ELO {
    public class SeedPlayers {
        private EventData EventData;
        public LeagueData LeagueData { get; }
        public RoundData SRCRoundData { get; }

        private ELOEngine ELO;

        public SeedPlayers(LeagueData leagueData, EventData eventData, RoundData roundData) {
            this.LeagueData = leagueData;
            this.EventData = eventData;
            this.SRCRoundData = roundData;
            this.ELO = new(leagueData);
        }

        public RoundData Run() {
            // The new round that will be returned
            RoundData newRoundData = new(this.EventData);

            // Create a list of names & generate ELO for this round
            List<string> names = [.. SRCRoundData.Records().Select(r => r.Name)];
            this.ELO = new(this.LeagueData);

            // Extra players
            List<string> setAside = [];

            // Make the ELOEngine list even
            if (names.Count % 2 != 0) {
                setAside = SetAside(3, names);

                // Assign those players to a 4321 match
                var matchData = newRoundData.AddMatch(new MatchData(newRoundData) {
                    MatchFormat = MatchFormat.A4321,
                    Ends = this.EventData.DefaultEnds
                });

                int i = 0;
                foreach (string name in setAside) {
                    matchData.Teams[i++].AddPlayer(name);
                }
            }

            this.AssignTeams(newRoundData);
            var matchesAssigned = this.AssignMatches(newRoundData);
            return matchesAssigned;
            
        }

        private RoundData AssignTeams(RoundData newRoundData) {
            Graph<string> graph = new();
            GreedyAugmenting<string> pairingAlgorithm = new(graph);

            // Build a graph, players are nodes, connected to players they can play with.
            // The edges are weighted with the elo differece between the two players.

            var sourceNames = this.SRCRoundData.AllNames();
            foreach (var forPlayer in sourceNames) {
                List<string> previousPartners = this.EventData.AllTeams()
                                                    .Where(t => t.Parent.Parent != this.SRCRoundData)
                                                    .Where(t => t.Players.Contains(forPlayer))
                                                    .SelectMany(t => t.Players)
                                                    .Where(n => n != forPlayer)
                                                    .Where(n => sourceNames.Contains(n))
                                                    .ToList();

                Debug.WriteLine($"ForPlayer = {forPlayer} : {previousPartners.JoinString()}");

                foreach (string otherPlayer in sourceNames) {
                    if (forPlayer == otherPlayer) continue;
                    if (previousPartners.Contains(otherPlayer)) continue;
                    int diff = ELO[forPlayer] - ELO[otherPlayer];
                    graph.AddEdge(forPlayer, otherPlayer, diff * diff);
                    Debug.WriteLine($"{forPlayer} <-> {otherPlayer} = {diff * diff}");
                }
            }

            Debug.WriteLine(graph);
            Debug.WriteLine($"Number of Nodes in Graph {graph.Nodes.Count}.");
            Debug.WriteLine($"Number of Edges in Graph {graph.Edges.Count()}.");

            // Run the pairing algorithm to pair players up.
            Solution<string>? best = pairingAlgorithm.Run(50, 50);
            if (best == null) throw new Exception("No pairings generated");

            Debug.WriteLine($"Fitness = {best.Fitness}");

            // Add matches to the round for each pairing (1 match for 2 pairs).
            int team = 0; 
            for (int i = 0; i < best!.Count; i++) {
                MatchData match = new(newRoundData) {
                    MatchFormat = MatchFormat.VS2,
                    Ends = this.EventData.DefaultEnds
                };

                Debug.WriteLine($"{best[i].Item1}, {best[i].Item2}, {graph.GetEdge(best[i].Item1, best[i].Item2).Weight}");
                match.Teams[team].CopyFrom([best[i].Item1, best[i].Item2]);
                newRoundData.AddMatch(match);
                team = team == 0 ? 1 : 0; // team flips between 0 and 1
            }

            return newRoundData;
        }

        private RoundData AssignMatches(RoundData roundData) {

            // Order teams by their collected ELOEngine
            List<TeamData> orderdTeams = 
                roundData.Matches
                .SelectMany(match => match.Teams)
                .Where(t => !t.IsEmpty())
                .OrderBy(t => this.ELO.GetELO(t.Players))
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
        /// SetPlayer aside 3 players to play 4-3-2-1
        /// </summary>
        /// TODO MAKE THIS NOT RANDOM
        private static List<string> SetAside(int count, List<string> names) {
            List<string> setAside = [];
            Random rng = new Random(123);

            while (setAside.Count < 3) {
                string name = names[rng.Next(names.Count)];
                names.Remove(name);
                setAside.Add(name);
            }

            return setAside;
        }
    }
}
