using Algorithms;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.ViewModel;
using System.Diagnostics;
using Utility;
using Utility.Extensions;

namespace Leagueinator.GUI.Controllers.Modules.AssignLanes {
    /// <summary>
    /// Provides logic to assign lanes to matches in a round, ensuring that teams are not repeatedly assigned to the same lanes across _rounds.
    /// </summary>
    public class AssignLanes {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssignLanes"/> class.
        /// </summary>
        /// <param name="eventData">The event configuration data.</param>
        /// <param name="rounds">The collection of all _rounds in the event.</param>
        /// <param name="roundData">The round to assign lanes for.</param>
        public AssignLanes(EventData eventData, RoundData roundData) {
            this.EventData = eventData;
            this.RoundData = roundData;
        }

        /// <summary>
        /// Gets the event data containing configuration such as forMatch count and default ends.
        /// </summary>
        private EventData EventData { get; }

        /// <summary>
        /// The round for which lanes are being assigned.
        /// </summary>
        private RoundData RoundData { get; }


        private DefaultDictionary<string, List<int>> PreviousLanes() {
            // A dictiony for each player -> the lanes they have played on
            DefaultDictionary<string, List<int>> previousLanes = new(key => []);

            foreach (PlayerRecord record in this.EventData.Records()) {
                if (record.Round == this.RoundData.Index) continue;
                previousLanes[record.Name].Add(record.Lane);
            }

            return previousLanes;
        }

        private DefaultDictionary<MatchData, List<int>> PermittedLanes(DefaultDictionary<string, List<int>> previousLanes) {
            // A dictiony for each match -> the lanes they can play on
            DefaultDictionary<MatchData, List<int>> permittedLanes = new(key => [.. Enumerable.Range(0, this.EventData.LaneCount - 1)]);

            var teams = this.RoundData.Matches
                                      .SelectMany(m => m.Teams)
                                      .ToList();

            foreach (TeamData teamData in this.RoundData.Matches.SelectMany(m => m.Teams)){
                if (teamData.CountPlayers() == 0) continue;
                foreach (string name in teamData.Names) {
                    var forMatch = teamData.Parent;
                    var newList = permittedLanes[forMatch].Except(previousLanes[name]).ToList();
                    permittedLanes[forMatch] = newList;
                }
            }

            return permittedLanes;
        }

        public RoundData Run() {
            var previousLanes = this.PreviousLanes();
            var permittedLanes = this.PermittedLanes(previousLanes);

            foreach (MatchData match in permittedLanes.Keys) {
                var lanes = permittedLanes[match];
                Debug.WriteLine($"{match.PlayerNames().JoinString()} ; {lanes.JoinString()}");
            }

            AssignValues<MatchData, int> assign = new(permittedLanes);
            var result = assign.Run() ?? throw new UnsolvableException("Could not assign lanes to all players");

            RoundData newRound = new(this.EventData);
            newRound.Fill();

            Debug.WriteLine("New Round");
            Debug.WriteLine(newRound);

            foreach (MatchData matchData in result.Keys) {
                var newMatch = matchData.Copy(newRound);
                int newLane = result[matchData];

                Debug.WriteLine($"NewLane {newLane}");

                newRound.InsertMatch(newLane, newMatch);
            }

            Debug.WriteLine("New Round");
            Debug.WriteLine(newRound);

            return newRound;
        }
    }
}
