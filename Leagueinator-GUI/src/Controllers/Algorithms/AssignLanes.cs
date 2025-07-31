using Algorithms;
using Leagueinator.GUI.Model;
using System.Diagnostics;
using Utility;
using Utility.Extensions;

namespace Leagueinator.GUI.Controllers.Algorithms {
    /// <summary>
    /// Provides logic to assign lanes to matches in a round, ensuring that teams are not repeatedly assigned to the same lanes across rounds.
    /// </summary>
    public class AssignLanes {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssignLanes"/> class.
        /// </summary>
        /// <param name="eventData">The event configuration data.</param>
        /// <param name="rounds">The collection of all rounds in the event.</param>
        /// <param name="roundData">The round to assign lanes for.</param>
        public AssignLanes(EventData eventData, RoundData roundData) {
            this.EventData = eventData;
            this.RoundData = roundData;
        }

        /// <summary>
        /// Gets the event data containing configuration such as lane count and default ends.
        /// </summary>
        private EventData EventData { get; }

        /// <summary>
        /// Gets the round for which lanes are being assigned.
        /// </summary>
        private RoundData RoundData { get; }      

        public RoundData Run() {
            DefaultDictionary<string, List<int>> playerPlayedLanes = new(key => []);

            foreach (Record record in this.EventData.Records()) {
                playerPlayedLanes[record.Name].Add(record.MatchData.Lane);
            }

            DefaultDictionary<MatchData, List<int>> permittedLanes = new(key => [.. Enumerable.Range(1, this.EventData.LaneCount)]);

            Debug.WriteLine(this.RoundData.Records().JoinString("\n"));
            var recordsForRound = this.RoundData.Records().Where(r => r.RoundData == this.RoundData);

            foreach (Record record in recordsForRound) {
                List<int> allowedLanes = permittedLanes[record.MatchData];
                allowedLanes.RemoveAll(lane => playerPlayedLanes[record.Name].Contains(lane));
            }

            AssignValues<MatchData, int> assign = new(permittedLanes);
            var result = assign.Run() ?? throw new AlgoLogicException("Could not assign lanes to all players");

            RoundData newRound = [];

            foreach (MatchData matchData in result.Keys) {
                var newMatch = matchData.Copy();                
                newMatch.Lane = result[matchData];
                newRound.Add(newMatch);
            }

            newRound.Fill(this.EventData);
            return newRound;
        }
    }
}
