
using System.Text.Json.Serialization;

namespace Leagueinator.GUI.Model {
    public class TeamData(MatchData matchData, int size) : IEquatable<TeamData>, IHasParent<MatchData> {

        private readonly string[] _names = [.. Enumerable.Repeat(string.Empty, size)];
        public IReadOnlyList<string> Names => _names;
                
        public int Index => this.Parent.Teams.ToList().IndexOf(this);

        public MatchData Parent { get; } = matchData;

        public void AddPlayer(string name) {
            if (this.IsFull()) throw new InvalidOperationException("TeamData is full");

            for (int i = 0; i < this._names.Length; i++) {
                if (string.IsNullOrEmpty(this.Names[i])) {
                    this._names[i] = name;
                    return;
                }
            }
        }

        internal void Set(int position, string name) {
            this._names[position] = name;
        }

        public void Clear() {
            for (int i = 0; i < this._names.Length; i++) {
                this._names[i] = string.Empty;
            }
        }

        public TeamData Copy() {
            var teamData = new TeamData(this.Parent, this._names.Length);
            for (int i = 0; i < this._names.Length; i++) {
                teamData._names[i] = this._names[i]; 
            }
            return teamData;
        }

        public void Remove(string name) {
            for (int i = 0; i < this._names.Length; i++) {
                if (this._names[i] == name) {
                    this._names[i] = string.Empty;
                }
            }
        }

        public bool Equals(TeamData? other) {
            if (other is null) {
                return false;
            }

            if (this._names.Length != other._names.Length) {
                return false;
            }

            foreach (string name in Names) {
                if (!other.Names.Contains(name)) {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode() {
            int hash = 17;
            foreach (var s in this.Names)
                hash = hash * 31 + (s?.GetHashCode() ?? 0);
            return hash;
        }

        public override string ToString() {
            return string.Join(", ", Names);
        }

        public int CountPlayers() {
            int count = 0;
            foreach (string name in this.Names) {
                if (!string.IsNullOrEmpty(name)) {
                    count++;
                }
            }
            return count;
        }

        public IEnumerator<string> GetEnumerator() {
            foreach (string name in this.Names) {
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
    }
}
