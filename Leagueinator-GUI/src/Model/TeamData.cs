
using Leagueinator.GUI.Model.Enums;
using Leagueinator.GUI.Model.ViewModel;
using System.IO;
using Utility.Extensions;

namespace Leagueinator.GUI.Model {
    public class TeamData(MatchData matchData, int size) : IEquatable<TeamData>, IHasParent<MatchData> {

        private readonly string[] _names = [.. Enumerable.Repeat(string.Empty, size)];

        public List<string> Names => [.. this._names];

        public int Index => this.Parent.Teams.ToList().IndexOf(this);

        public MatchData Parent { get; } = matchData;

        public Players ToPlayers() => this._names
                                          .Select(p => p.Trim())
                                          .Where(p => !string.IsNullOrEmpty(p))
                                          .ToPlayers();

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
                return (this.Parent.TieBreaker == this.Index) ? GameResult.Win : GameResult.Tie;
            }
        }

        public bool TieBreaker {
            get => this.Parent.TieBreaker == this.Index;
        }

        public bool HasPlayer(string name) {
            return this._names.Contains(name);
        }

        public void AddPlayer(string name) {
            if (this.IsFull()) throw new InvalidOperationException("TeamData is full");

            for (int i = 0; i < this._names.Length; i++) {
                if (string.IsNullOrEmpty(this.Names[i])) {
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
            return this.Equals(other.Names);
        }

        public override int GetHashCode() {
            int hash = 17;
            foreach (var s in this.Names)
                hash = hash * 31 + (s?.GetHashCode() ?? 0);
            return hash;
        }

        public override string ToString() {
            return $"{this._names.JoinString()}";
        }

        public int CountPlayers() => this.ToPlayers().Count;

        internal bool IsEmpty() {
            return this.CountPlayers() == 0;
        }

        internal void CopyFrom(TeamData teamData) {
            var src = teamData._names;
            var dest = this._names;
            Array.Copy(src, dest, Math.Min(src.Length, dest.Length));
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
            string[] s = [this.Parent.Score[this.Index].ToString(), .. this._names];
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
    }
}
