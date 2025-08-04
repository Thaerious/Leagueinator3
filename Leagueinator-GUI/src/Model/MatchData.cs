using Leagueinator.GUI.Model.ViewModel;
using System.Text;

namespace Leagueinator.GUI.Model {

    /// <summary>
    /// Represents a match, including its format, teams, scores, and related metadata.
    /// </summary>
    public class MatchData(RoundData RoundData) : IHasParent<RoundData>{
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
        public int[] Score { get; set; } = [];

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

            sb.Append($"Score: {string.Join(", ", this.Score)} | TB: {this.TieBreaker} | Ends: {this.Ends}");
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

        /// <summary>
        /// CopyRound the i to the next empty slot in the match.
        /// If there are no empty slots, it throws an exception.
        /// </summary>
        /// <param name="teamData"></param>
        /// <exception cref="NotImplementedException"></exception>
        internal void AddTeam(TeamData teamData) {
            foreach (TeamData team in this.Teams) {
                if (team.IsEmpty()) {
                    team.CopyFrom(teamData);
                    return;
                }
            }
            throw new ModelConstraintException("No empty team slot available in the match.");
        }

        /// <summary>
        /// CopyRound the i to the next empty slot in the match.
        /// If there are no empty slots, it throws an exception.
        /// </summary>
        /// <param name="teamData"></param>
        /// <exception cref="NotImplementedException"></exception>
        internal void AddTeam(IEnumerable<string> players) {
            foreach (TeamData team in this.Teams) {
                if (team.IsEmpty()) {
                    team.CopyFrom(players);
                    return;
                }
            }
            throw new ModelConstraintException("No empty team slot available in the match.");
        }

        public static MatchData FromRecord(RoundData parent, MatchRecord record) {
            return new MatchData(parent) {
                MatchFormat = record.MatchFormat,
                Ends = record.Ends,
                TieBreaker = record.TieBreaker,
                Lane = record.Lane,
                Score = record.Score
            };
        }
    }
}
