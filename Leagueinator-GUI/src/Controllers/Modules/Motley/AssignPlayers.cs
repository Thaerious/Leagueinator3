using Algorithms;
using Algorithms.PairMatching;
using Leagueinator.GUI.Controllers.Algorithms;
using Leagueinator.GUI.Model;
using System.Diagnostics;
using Utility.Extensions;

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
            RoundData roundData = [];
            this.ELO = Motley.ELO.CalculateELO(this.LeagueData);
            List<string> names = [.. RoundData.Records().Select(r => r.Name)];
            List<string> setAside = [];

            // Keep only players from the source round (ELO has all players from the league).
            foreach (var name in this.ELO.Keys.ToList()) {
                if (!names.Contains(name)) {
                    this.ELO.Remove(name);
                }
            }

            if (ELO.Count % 2 != 0) {
                setAside = this.SetAside(3);
                foreach (string name in setAside) this.ELO.Remove(name);

                roundData.Add(new MatchData(MatchFormat.A4321) {
                    Lane = roundData.Count,
                    Ends = this.EventData.DefaultEnds
                });
            }

            if (ELO.Count % 4 != 0) {
                setAside = this.SetAside(2);
                foreach (string name in setAside) this.ELO.Remove(name);
                this.AssignTeams(roundData);
            }
            else {
                this.AssignTeams(roundData);
            }

            var matchesAssigned = this.AssignMatches(roundData);
            return matchesAssigned;
            
        }

        private RoundData AssignMatches(RoundData roundData) {
            Dictionary<TeamData, int> teamELO = [];

            foreach (TeamData team in roundData.Teams) {
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

            RoundData newRound = [];
            for (int i = 0; i < orderdTeams.Count; i += 2) {
                MatchData matchData = new(MatchFormat.VS2) {
                    Lane = newRound.Count,
                    Ends = this.EventData.DefaultEnds
                };
                matchData.AddTeam(orderdTeams[i]);
                matchData.AddTeam(orderdTeams[i + 1]);
                newRound.Add(matchData);
            }

            return newRound;

        }

        private RoundData AssignTeams(RoundData roundData) {
            Graph<string> graph = new();
            GreedyAugmenting<string> pairingAlgorithm = new(graph);

            foreach (var kv1 in this.ELO) {
                string forPlayer = kv1.Key;

                List<string> previousPartners = this.EventData.Records()
                                                    .Where(r => r.Name == forPlayer)
                                                    .Select(r => r.TeamData)
                                                    .SelectMany(t => t.Names)
                                                    .Where(name => name != forPlayer)
                                                    .ToList();

                foreach (var kv2 in this.ELO) {
                    if (kv1.Key == kv2.Key) continue;
                    if (!previousPartners.Contains(kv2.Key)) {
                        graph.AddEdge(kv1.Key, kv2.Key, Math.Abs(kv1.Value - kv2.Value));
                    }
                }
            }

            Solution<string>? best = pairingAlgorithm.Run(20, 20);

            if (best == null) throw new Exception("No pairings generated");

            for (int i = 0; i < best!.Count; i += 2) {
                MatchData match = new(MatchFormat.VS2) {
                    Lane = roundData.Count,
                    Ends = this.EventData.DefaultEnds
                };

                match.Teams[0].CopyFrom([best[i].Item1, best[i].Item2]);
                match.Teams[1].CopyFrom([best[i + 1].Item1, best[i + 1].Item2]);
                roundData.Add(match);
            }

            return roundData;
        }

        /// <summary>
        /// Set aside 3 players to play 4-3-2-1
        /// </summary>
        private List<string> SetAside(int count) {
            Dictionary<string, int> localELO = new(this.ELO);

            this.EventData
                .Records()
                .Where(r => r.MatchData.MatchFormat == MatchFormat.A4321)
                .ToList().ForEach(r => {
                    localELO.Remove(r.Name);
                });

            var sortedList = localELO.OrderBy(kv => kv.Value).Select(kv => kv.Key).ToList();
            sortedList.RemoveRange(0, sortedList.Count - count);
            return sortedList;
        }

        internal static RoundData Assign(LeagueData leagueData, EventData eventData, RoundData roundData) {
            AssignPlayers assignPlayers = new(leagueData, eventData, roundData);
            return assignPlayers.Run();
        }
    }
}
