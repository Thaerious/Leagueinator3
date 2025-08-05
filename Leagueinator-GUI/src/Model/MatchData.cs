using Leagueinator.GUI.Model.ViewModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Leagueinator.GUI.Model {

    /// <summary>
    /// Represents a match, including its format, teams, scores, and related metadata.
    /// </summary>
    public class MatchData(RoundData RoundData) : IHasParent<RoundData> {
        private MatchFormat _matchFormat = MatchFormat.VS2;

        public RoundData Parent { get; } = RoundData;

        private readonly List<TeamData> _teams = [];
        public IReadOnlyList<TeamData> Teams => _teams;

        /// <summary>
        /// Gets or sets the match format, which determines the number of teams and i size.
        /// Changing the format resets the teams and scores.
        /// </summary>
        public required MatchFormat MatchFormat {
            get => this._matchFormat;
            set {
                var teamCount = value.TeamCount();
                var teamSize = value.TeamSize();

                this._matchFormat = value;
                this.Score = new int[teamCount];

                this._teams.Clear();
                while (this._teams.Count < teamCount) {
                    this._teams.Add(new TeamData(this, teamSize));
                }

                for (int i = 0; i < this.Score.Length; i++) {
                    this.Score[i] = 0;
                }
            }
        }

        /// <summary>
        /// Gets or sets the lane number for the match. Default is -1 (unassigned).
        /// </summary>
        public int Lane { get; set; } = -1;

        /// <summary>
        /// Gets or sets the number of ends (_rounds/segments) played in the match.
        /// </summary>
        public int Ends { get; set; } = 0;

        /// <summary>
        /// Gets or sets the scores for each i in the match.
        /// </summary>
        public int[] Score { get; set; } = [];  // TODO Probably should be on team data, can create this in Linq

        /// <summary>
        /// Gets or sets the tiebreaker value for the match. Default is -1 (no tiebreaker).
        /// </summary>
        public int TieBreaker { get; set; } = -1;

        /// <summary>
        /// Creates a deep copy of this <see cref="MatchData"/> instance, including teams and scores.
        /// </summary>
        /// <returns>A new <see cref="MatchData"/> object with the same data.</returns>
        public MatchData Copy() {
            MatchData matchCopy = new(this.Parent) {
                MatchFormat = this.MatchFormat,
                Lane = this.Lane,
                Ends = this.Ends,
            };

            // Deep copy Teams
            for (int i = 0; i < this.Teams.Count; i++) {
                matchCopy.Teams[i].CopyFrom(this.Teams[i]);
            }

            return matchCopy;
        }

        /// <summary>
        /// Counts the total number of players across all teams in the match.
        /// </summary>
        /// <returns>The total number of players.</returns>
        public int CountPlayers() {
            int count = 0;
            foreach (TeamData team in this.Teams) {
                count += team.CountPlayers();
            }
            return count;
        }

        /// <summary>
        /// Returns a string representation of the match, including lane, format, teams, scores, tiebreaker, and ends.
        /// </summary>
        /// <returns>A string describing the match.</returns>
        public override string ToString() {
            StringBuilder sb = new();
            sb.Append($"Match {this.Lane}: {this.MatchFormat} | Players: ");

            foreach (TeamData team in this.Teams) {
                sb.Append($"[{team}]");
            }

            sb.Append($" | Score: {string.Join(", ", this.Score)} | TB: {this.TieBreaker} | Ends: {this.Ends}");
            return sb.ToString();
        }

        /// <summary>
        /// Removes a player with the specified name from all teams in the match.
        /// </summary>
        /// <param name="name">The name of the player to remove.</param>
        public void RemoveName(string name) {
            foreach (TeamData team in this.Teams) {
                team.Remove(name);
            }
        }

        internal List<string> PlayerNames() {
            return [.. this.Teams.SelectMany(t => t.Names).Where(name => !string.IsNullOrEmpty(name))];
        }

        public IEnumerable<PlayerRecord> Records() {
            return this.Teams.SelectMany(team => team.Records());
        }

        internal void WriteOut(StreamWriter writer) {
            writer.WriteLine(
                string.Join("|",
                    this.MatchFormat,
                    this.Ends,
                    this.Lane,
                    this.TieBreaker,
                    this.Teams.Count
                )
            );

            foreach (TeamData teamData in this.Teams) {
                teamData.WriteOut(writer);
            }
        }

        internal static MatchData ReadIn(RoundData roundData, StreamReader reader) {
            string? line = reader.ReadLine() ?? throw new EndOfStreamException("Unexpected end of file while reading EventData");
            string[] parts = line.Split('|');
            if (parts.Length != 5) throw new FormatException("Invalid EventData format");

            MatchData matchData = new(roundData) {
                MatchFormat = Enum.Parse<MatchFormat>(parts[0]),
                Ends        = int.Parse(parts[1]),
                Lane        = int.Parse(parts[2]),
                TieBreaker  = int.Parse(parts[3]),
            };

            int teamCount = int.Parse(parts[4]);
            for (int i = 0; i < teamCount; i++) {
                TeamData teamData = TeamData.ReadIn(matchData, i, reader);
                matchData._teams[i] = teamData;
            }

            return matchData;
        }
    }
}

