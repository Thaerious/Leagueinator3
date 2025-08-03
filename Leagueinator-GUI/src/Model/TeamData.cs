
using System.Text.Json.Serialization;

namespace Leagueinator.GUI.Model {
    public class TeamData : IEquatable<TeamData> {

        public string[] Names { get; }
        public int Length => this.Names.Length;
        public int Index { get; }
        public Players Players => [.. this.Names]; // TODO Names should not be public.

        public TeamData(int size, int index) {
            this.Names = new string[size];
            Array.Fill(this.Names, string.Empty);
            this.Index = index;
        }

        [JsonConstructor]
        public TeamData(string[] names) {
            this.Names = [.. names];
        }

        public void AddPlayer(string name) {
            if (this.IsFull()) throw new InvalidOperationException("TeamData is full");
            
            for (int i = 0; i < this.Names.Length; i++) {
                if (string.IsNullOrEmpty(this.Names[i])) {
                    this.Names[i] = name;
                    return;
                }
            }
        }

        public void Clear() {
            for (int i = 0; i < this.Names.Length; i++) {
                this.Names[i] = string.Empty;
            }
        }

        public TeamData Copy() {
            return new TeamData(this.Names);
        }

        public string this[int index] {
            get {
                if (index < 0 || index >= this.Names.Length) {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
                }
                return this.Names[index];
            }
            set {
                if (index < 0 || index >= this.Names.Length) {
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
                }
                this.Names[index] = value;
            }
        }

        public int IndexOf(string name) {
            return Array.IndexOf(this.Names, name);
        }

        public void Remove(string name) {
            for (int i = 0; i < this.Names.Length; i++) {
                if (this.Names[i] == name) {
                    this.Names[i] = string.Empty;
                }
            }
        }

        public bool Equals(TeamData? other) {
            if (other is null) {
                return false;
            }

            if (this.Names.Length != other.Names.Length) {
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

        internal int CountPlayers() {
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
            teamData.Names.CopyTo(this.Names, 0);
        }

        internal void CopyFrom(IEnumerable<string> players) {
            players.ToArray().CopyTo(this.Names, 0);
        }

        internal bool IsFull() {
            return this.CountPlayers() == this.Names.Length;
        }

        internal void SetNames(List<string> names) {
            this.Clear();
            names.CopyTo(this.Names);   
        }
    }
}
