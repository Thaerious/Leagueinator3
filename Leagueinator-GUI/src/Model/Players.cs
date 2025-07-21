
namespace Leagueinator.GUI.Model {
    public class Players : List<string>, IEquatable<Players> {
        
        public Players(): base() { }
        
        public Players(IEnumerable<string> source) : base(source) {
            this.AddRange(source);
        }

        public bool Equals(Players? other) {
            if (other is null) {
                return false;
            }

            if (this.Count != other.Count) {
                return false;
            }

            foreach (string name in this) {
                if (!other.Contains(name)) {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object? obj) {
            return Equals(obj as Players);
        }

        public override int GetHashCode() {
            int hash = 17;
            foreach (var s in this)
                hash = hash * 31 + (s?.GetHashCode() ?? 0);
            return hash;
        }
    }
}
