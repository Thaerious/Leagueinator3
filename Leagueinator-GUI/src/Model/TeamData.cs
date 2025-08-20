
using Leagueinator.GUI.Model.Enums;
using Leagueinator.GUI.Model.ViewModel;
using System.Diagnostics;
using System.IO;
using Utility.Extensions;

namespace Leagueinator.GUI.Model {
    public class TeamData(MatchData matchData, int size) : IEquatable<TeamData>, IHasParent<MatchData> {

        private readonly string[] _names = [.. Enumerable.Repeat(string.Empty, size)];

        public IReadOnlyList<string> Players => [.. this._names.Select(n => n.Trim()).Where(n => !string.IsNullOrEmpty(n))];

        public int Index => this.Parent.Teams.ToList().IndexOf(this);

        public MatchData Parent { get; } = matchData;

        public int ShotsFor {
            get => this.Parent.Score[this.Index];
            set => this.Parent.Score[this.Index] = value;
        }

        public int ShotsAgainst => this.Parent.Teams.Where(t => t != this).Select(t => t.ShotsFor).Sum();

        public GameResult Result {
            get {
                var teams = this.Parent.Teams.Where(t => !t.IsEmpty());

                // No scores registered, implies game not played.
                if (teams.All(t => t.ShotsFor <= 0)) return GameResult.Vacant;

                // Team does not have the max score
                int max = teams.Max(t => t.ShotsFor);
                if (this.ShotsFor < max) return GameResult.Loss;

                // Team shares max score
                bool multipleAtMax = teams.Count(t => t.ShotsFor == max) > 1;
                if (!multipleAtMax) return GameResult.Win;

                // Tie at max: resolve with tiebreaker
                return (this.Parent.TieBreaker == this.Index) ? GameResult.Win : GameResult.Draw;
            }
        }

        public bool HasPlayer(string name) {
            return this._names.Contains(name);
        }

        public void AddPlayer(string name) {
            if (this.IsFull()) throw new InvalidOperationException("TeamData is full");

            for (int i = 0; i < this._names.Length; i++) {
                if (string.IsNullOrEmpty(this.Players[i])) {
                    this._names[i] = name;
                    return;
                }
            }
        }

        internal void SetPlayer(int position, string name) {
            this._names[position] = name;
        }

        public void ClearPlayers() {
            for (int i = 0; i < this._names.Length; i++) {
                this._names[i] = string.Empty;
            }
        }
        public void RemovePlayer(string name) {
            for (int i = 0; i < this._names.Length; i++) {
                if (this._names[i] == name) {
                    this._names[i] = string.Empty;
                }
            }
        }

        public TeamData Copy() {
            var teamData = new TeamData(this.Parent, this._names.Length);
            for (int i = 0; i < this._names.Length; i++) {
                teamData._names[i] = this._names[i];
            }
            return teamData;
        }

        /// <summary>
        /// Return true if the names in this team matches the enumerable exactly without
        /// leftover, and w/o consideration to order.
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public bool Equals(IEnumerable<string> names) {
            if (names is null) return false;

            if (this._names.Length != names.Count()) return false;

            foreach (string name in names) {
                if (!this._names.Contains(name)) return false;
            }

            return true;
        }

        public bool Equals(TeamData? other) {
            if (other is null) return false;
            return this.Equals(other.Players);
        }

        public override int GetHashCode() {
            int hash = 17;
            foreach (var s in this.Players)
                hash = hash * 31 + (s?.GetHashCode() ?? 0);
            return hash;
        }

        public override string ToString() {
            return $"[{this._names.JoinString()}]:{this.Result}";
        }

        public int CountPlayers() {
            int count = 0;
            foreach (string name in this.Players) {
                if (!string.IsNullOrEmpty(name)) {
                    count++;
                }
            }
            return count;
        }

        public IEnumerator<string> GetEnumerator() {
            foreach (string name in this.Players) {
                if (name == string.Empty) continue;
                yield return name;
            }
        }

        internal bool IsEmpty() {
            return this.CountPlayers() == 0;
        }

        internal void CopyFrom(TeamData teamData) {
            teamData._names.CopyTo(this._names, 0);
        }

        internal void CopyFrom(IEnumerable<string> players) {
            players.ToArray().CopyTo(this._names, 0);
        }

        internal bool IsFull() {
            return this.CountPlayers() == this._names.Length;
        }

        public IEnumerable<PlayerRecord> Records() {
            for (int i = 0; i < this._names.Length; i++) {
                string name = this._names[i];
                if (string.IsNullOrEmpty(name)) continue;
                yield return new PlayerRecord(this, i);
            }
        }

        internal void WriteOut(StreamWriter writer) {
            string[] s = [this.Parent.Score[this.Index].ToString(), ..this.Players];
            writer.WriteLine(string.Join("|", s));
        }

        internal static TeamData ReadIn(MatchData matchData, int index, StreamReader reader) {
            string? line = reader.ReadLine() ?? throw new EndOfStreamException("Unexpected end of file while reading EventData");
            string[] parts = line.Split('|');
            if (parts.Length != matchData.MatchFormat.TeamSize() + 1) throw new FormatException("Invalid EventData format");

            TeamData teamData = new(matchData, matchData.MatchFormat.TeamSize());

            for (int i = 1; i < parts.Length; i++) {
                teamData.SetPlayer(i - 1, parts[i]);
            }

            matchData.Score[index] = int.Parse(parts[0]);
            return teamData;
        }

        public bool HasPlayers() {
            return this.CountPlayers() > 0;
        }
    }
}
