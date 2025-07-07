using System.Text;

namespace Leagueinator.GUI.Model {
    /// <summary>
    /// Represents a match, including its format, teams, scores, and related metadata.
    /// </summary>
    public class MatchData {
        private MatchFormat _matchFormat = MatchFormat.VS2;

        /// <summary>
        /// Gets or sets the match format, which determines the number of teams and team size.
        /// Changing the format resets the teams and scores.
        /// </summary>
        public MatchFormat MatchFormat { 
            get => this._matchFormat;
            set {
                var teamCount = MatchFormatMeta.Info[value].TeamCount;
                var teamSize = MatchFormatMeta.Info[value].TeamSize;

                this._matchFormat = value;
                this.Score = new int[teamCount];
                var OldPlayers = this.Teams;
                this.Teams = TeamData.Collection(teamCount, teamSize);

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
        /// Gets or sets the number of ends (rounds/segments) played in the match.
        /// </summary>
        public int Ends { get; set; } = 0;

        /// <summary>
        /// Gets or sets the scores for each team in the match.
        /// </summary>
        public int[] Score { get; set; } = [];

        /// <summary>
        /// Gets or sets the collection of teams participating in the match.
        /// </summary>
        public TeamData[] Teams { get; set; } = [];

        /// <summary>
        /// Gets or sets the tiebreaker value for the match. Default is -1 (no tiebreaker).
        /// </summary>
        public int TieBreaker { get; set; } = -1;
        public List<string> Players { 
            get => this.Teams.SelectMany(team => team.Names).ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchData"/> class with the specified match format.
        /// </summary>
        /// <param name="matchFormat">The format of the match (number of teams and team size).</param>
        public MatchData(MatchFormat matchFormat) {
            var teamCount = MatchFormatMeta.Info[matchFormat].TeamCount;
            var teamSize = MatchFormatMeta.Info[matchFormat].TeamSize;

            this.MatchFormat = matchFormat;
            this.Score = new int[teamCount];
            this.Teams = TeamData.Collection(teamCount, teamSize);

            for (int i = 0; i < this.Score.Length; i++) {
                this.Score[i] = 0;
            }
        }

        /// <summary>
        /// CopyRound the team to the specified index in the match.
        /// </summary>
        /// <param name="teamIndex">The index of the team to set.</param>
        /// <param name="team">The team data to assign.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the team index is out of range.</exception>
        public void SetTeam(int teamIndex, TeamData team) {
            if (teamIndex < 0 || teamIndex >= this.Teams.Length) {
                throw new ArgumentOutOfRangeException(nameof(teamIndex), "Team index is out of range.");
            }
            this.Teams[teamIndex] = team.Copy();
        }   

        /// <summary>
        /// Creates a deep copy of this <see cref="MatchData"/> instance, including teams and scores.
        /// </summary>
        /// <returns>A new <see cref="MatchData"/> object with the same data.</returns>
        public MatchData Copy() {
            MatchData matchCopy = new(this.MatchFormat) {
                Lane = this.Lane,
                Ends = this.Ends,
            };

            // Deep copy Teams
            for (int team = 0; team < this.Teams.Length; team++) {
                matchCopy.Teams[team] = this.Teams[team].Copy();
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
        /// Counts the number of teams that have at least one player.
        /// </summary>
        /// <returns>The number of non-empty teams.</returns>
        public int CountTeams() {
            int count = 0;

            foreach(TeamData team in this.Teams) {
                if (team.CountPlayers() > 0) {
                    count++;
                }
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
        /// CopyRound the team to the next empty slot in the match.
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
    }
}
