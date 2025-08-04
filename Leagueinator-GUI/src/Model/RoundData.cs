using Utility.Extensions;
using System.Diagnostics;
using System.Text;
using Leagueinator.GUI.Model.ViewModel;

namespace Leagueinator.GUI.Model {
    public record PlayerLocation(int Lane, int TeamIndex, int Position);

    public class RoundData(EventData EventData) : object(), IHasParent<EventData> {

        public int Index => this.Parent.Rounds.ToList().IndexOf(this);

        private readonly List<MatchData> _matches = [];
        public IReadOnlyList<MatchData> Matches => _matches;

        public EventData Parent { get; } = EventData;

        public void Fill() {
            for (int i = 0; i < this.Parent.DefaultLaneCount; i++) {
                if (!this._matches.Any(m => m.Lane == i)) {
                    this._matches.Add(
                        new MatchData(this) {
                            MatchFormat = this.Parent.DefaultMatchFormat,
                            Lane = i,
                            Ends = this.Parent.DefaultEnds
                        }
                    );
                }
            }

            this._matches.Sort((a, b) => a.Lane.CompareTo(b.Lane));
        }

        public RoundData Copy() {
            RoundData roundCopy = new(this.Parent);

            foreach (MatchData match in this._matches) {
                roundCopy._matches.Add(match.Copy());
            }

            return roundCopy;
        }

        public void RemovePlayer(string name) {
            if (string.IsNullOrEmpty(name)) return;

            foreach (MatchData match in this._matches) {
                match.RemoveName(name);
            }
        }

        public bool HasPlayer(string name) {
            if (string.IsNullOrEmpty(name)) return false;

            foreach (MatchData match in this._matches) {
                foreach (TeamData team in match.Teams) {
                    if (team.Names.Contains(name)) {
                        return true;
                    }
                }
            }
            return false;
        }

        public IEnumerable<PlayerRecord> Records() {
            return this.Parent.Records().Where(r => r.Round == this.Index);
        }

        public override string ToString() {
            StringBuilder sb = new();
            foreach (MatchData match in this._matches) {
                sb.Append(match.ToString());
                sb.Append('\n');
            }
            return sb.ToString();
        }
    }
}
