using Leagueinator.GUI.Model.ViewModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using Utility.Extensions;

namespace Leagueinator.GUI.Model {

    public class RoundData(EventData EventData) : object(), IHasParent<EventData> {

        public int Index => this.Parent.Rounds.ToList().IndexOf(this);

        private readonly List<MatchData> _matches = [];
        public IReadOnlyList<MatchData> Matches => _matches;

        public EventData Parent { get; } = EventData;

        public RoundData Fill(int? toIndex = null) {
            toIndex = toIndex ?? this.Parent.LaneCount;

            while (this._matches.Count < toIndex) {
                this._matches.Add(
                    new MatchData(this) {
                        MatchFormat = this.Parent.DefaultMatchFormat,
                        Ends = this.Parent.DefaultEnds
                    }
                );
            }

            return this;
        }

        public void Trim() {
            List<MatchData> matches = [.. this._matches];
            foreach (var match in matches) {
                if (match.CountPlayers() == 0) {
                    this.RemoveMatch(match);
                }
            }
        }

        public RoundData Copy() {
            RoundData roundCopy = new(this.Parent);

            foreach (MatchData match in this._matches) {
                roundCopy._matches.Add(match.Copy(roundCopy));
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
            return this.Matches.SelectMany(match => match.Records());
        }

        public override string ToString() {
            StringBuilder sb = new();
            foreach (MatchData match in this._matches) {
                sb.Append(match.ToString());
                sb.Append('\n');
            }
            return sb.ToString();
        }

        internal void WriteOut(StreamWriter writer) {
            writer.WriteLine(this.Matches.Count);
            foreach (MatchData match in this._matches) {
                match.WriteOut(writer);
            }
        }

        internal static RoundData ReadIn(EventData eventData, StreamReader reader) {
            RoundData roundData = new(eventData);
            int matchCount = int.Parse(reader?.ReadLine() ?? throw new FormatException("Invalid save format"));

            for (int i = 0; i < matchCount; i++) {
                MatchData matchData = MatchData.ReadIn(roundData, reader);
                roundData._matches.Add(matchData);
            }
            return roundData;
        }

        internal void RemoveMatch(MatchData match) {
            this._matches.Remove(match);
        }

        internal void SwapMatch(int lane1, int lane2) {
            (this._matches[lane2], this._matches[lane1]) = (this._matches[lane1], this._matches[lane2]);
        }

        internal void ReplaceMatch(int index, MatchData match) {
            if (match.Parent != this) throw new InvalidParentException();
            this._matches[index] = match;
        }

        internal MatchData AddMatch(MatchData match) {
            if (match.Parent != this) throw new InvalidParentException();
            this._matches.Add(match);
            return match;
        }

        internal void AddMatch() {
            MatchData match = new(this) {
                MatchFormat = this.Parent.DefaultMatchFormat,
                Ends = this.Parent.DefaultEnds
            };
            this._matches.Add(match);
        }

        internal IEnumerable<string> AllNames() {
            return this.Matches.SelectMany(m => m.AllNames());
        }

        internal IEnumerable<TeamData> AllTeams() {
            return this.Matches.SelectMany(m => m.Teams);
        }
    }
}
