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
        public RoundData RoundData { get; }

        private ELOEngine ELO;

        public SeedPlayers(RoundData roundData) {
            this.RoundData = roundData;
            this.EventData = roundData.Parent;
            this.LeagueData = this.EventData.Parent;            
            this.ELO = new(this.LeagueData);
        }

        public RoundData NewRound() {
            // Create a list of names & generate ELO for this round
            List<string> names = [.. this.EventData.AllNames()];
            this.ELO = new(this.LeagueData);

            // Extra players
            List<string> setAside = [];

            // Make the ELOEngine list even
            if (names.Count % 2 != 0) {
                setAside = SetAside(1, names);
            }

            RoundData newRoundData = this.AssignTeams(names);
            var matchesAssigned = this.AssignMatches(newRoundData, setAside);
            ReplaceSetAside(matchesAssigned, setAside[0]);
            return matchesAssigned;            
        }

        private RoundData AssignTeams(List<string> names) {
            Debug.WriteLine(names.JoinString());
            Debug.WriteLine($"{names.Count} names");

            Graph<string> graph = new();
            GreedyAugmenting<string> pairingAlgorithm = new(graph);

            // Build a graph, players are nodes, connected to players they can play with.
            // The edges are weighted with the elo differece between the two players.
            foreach (var forPlayer in names) {
                List<string> previousPartners = this.EventData.AllTeams()
                                                    .Where(t => t.Parent.Parent != this.RoundData)
                                                    .Where(t => t.Players.Contains(forPlayer))
                                                    .SelectMany(t => t.Players)
                                                    .Where(n => n != forPlayer)
                                                    .Where(n => names.Contains(n))
                                                    .ToList();

                Debug.WriteLine($"ForPlayer = {forPlayer} : {previousPartners.JoinString()}");

                foreach (string otherPlayer in names) {
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

            // NewRound the pairing algorithm to pair players up.
            Solution<string>? best = pairingAlgorithm.Run(50, 50);
            if (best == null) throw new Exception("No pairings generated");

            Debug.WriteLine($"Fitness = {best.Fitness}");

            // Add matches to the round for each pairing (1 match for 2 pairs).
            RoundData newRoundData = new(this.EventData);
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

        private RoundData AssignMatches(RoundData roundData, List<string> setAside) {

            // Order teams by their collected ELOEngine
            List<TeamData> orderedTeams = 
                roundData.Matches
                .SelectMany(match => match.Teams)
                .Where(t => !t.IsEmpty())
                .OrderBy(t => this.ELO.GetELO(t.Players))
                .ToList();

            RoundData newRound = new(this.EventData);

            while (orderedTeams.Count > 0) {
                MatchData matchData = new(newRound) {
                    MatchFormat = MatchFormat.VS2,
                    Ends = this.EventData.DefaultEnds
                };

                newRound.AddMatch(matchData);

                var next = orderedTeams.First();
                orderedTeams.Remove(next);
                matchData.Teams[0].CopyFrom(next);

                if (orderedTeams.Count == 0) break;

                next = orderedTeams.First();
                orderedTeams.Remove(next);
                matchData.Teams[1].CopyFrom(next);
            }

            return newRound;
        }

        /// <summary>
        /// SetPlayer aside 3 players to play 4-3-2-1
        /// </summary>
        /// TODO MAKE THIS NOT RANDOM
        private static List<string> SetAside(int count, List<string> names) {
            List<string> setAside = [];
            Random rng = new Random();

            while (setAside.Count < count) {
                string name = names[rng.Next(names.Count)];
                names.Remove(name);
                setAside.Add(name);
            }

            return setAside;
        }

        private void ReplaceSetAside(RoundData roundData, string name) {
            int targetELO = this.ELO[name];
            TeamData bestTeam = roundData.AllTeams().First();
            int bestELO = (int)bestTeam.Players.Select(p => this.ELO[p]).Average();

            foreach (TeamData team in roundData.AllTeams()) {
                int teamELO = (int)team.Players.Select(p => this.ELO[p]).Average();
                if (Math.Abs(teamELO - targetELO) < Math.Abs(teamELO - bestELO)) {
                    bestTeam = team;
                    bestELO = (int)bestTeam.Players.Select(p => this.ELO[p]).Average();
                }
            }

            MatchData matchData = new MatchData(roundData) {
                MatchFormat = MatchFormat.VS3,
                Ends = this.EventData.DefaultEnds
            };

            matchData.Teams[0].CopyFrom(bestTeam);
            matchData.Teams[0].AddPlayer(name);
            matchData.Teams[1].CopyFrom(bestTeam.GetOpposition().First());
            roundData.ReplaceMatch(bestTeam.Parent.Lane, matchData);
        }
    }
}
