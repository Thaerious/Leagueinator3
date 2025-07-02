
using System.Collections;
using System.Text.Json.Serialization;

namespace Leagueinator.GUI.Model {
    public class TeamData : IEquatable<TeamData> {

        public string[] Names { get; }

        public TeamData(int size) {
            this.Names = new string[size];
            Array.Fill(this.Names, string.Empty);
        }

        [JsonConstructor]
        public TeamData(string[] names) {
            this.Names = [.. names];
        }

        public TeamData Copy() {
            return new TeamData(this.Names);
        }

        public void Clear() {
            Array.Fill(this.Names, string.Empty);
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

        public void Add(string name, int position) {
            if (position < 0 || position >= this.Names.Length) {
                throw new ArgumentOutOfRangeException(nameof(position), "Position is out of range.");
            }
            this.Names[position] = name;
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

        public static TeamData[] Collection(int teamCount, int teamSize) {
            TeamData[] teams = new TeamData[teamCount];
            for (int i = 0; i < teamCount; i++) {
                teams[i] = new TeamData(teamSize);
            }
            return teams;
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

        public int Length => this.Names.Length;

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
    }
}
