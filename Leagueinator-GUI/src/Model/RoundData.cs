using Utility.Extensions;
using System.Diagnostics;
using System.Text;

namespace Leagueinator.GUI.Model {
    public record PlayerLocation(int Lane, int TeamIndex, int Position);

    public class RoundData : List<MatchData> {

        public RoundData() : base() { }

        public void Fill(EventData eventData) {
            for (int i = 0; i < eventData.LaneCount; i++) {
                if (!this.Any(m => m.Lane == i)) {
                    this.Add(new MatchData() {
                        MatchFormat = eventData.MatchFormat,
                        Lane = i,
                        Ends = eventData.DefaultEnds
                    });
                }
            }

            this.Sort((a, b) => a.Lane.CompareTo(b.Lane));
        }

        public void ClearNames() {
            foreach (MatchData matchData in this) {
                foreach (TeamData teamData in matchData.Teams) {
                    teamData.Clear();
                }
            }
        }

        public RoundData Copy() {
            RoundData roundCopy = [];

            foreach (MatchData match in this) {
                roundCopy.Add(match.Copy());
            }

            return roundCopy;
        }

        public void RemovePlayer(string name) {
            if (string.IsNullOrEmpty(name)) {
                return;
            }

            foreach (MatchData match in this) {
                match.RemoveName(name);
            }
        }

        public void SetPlayer(string name, int lane, int teamIndex, int position) {
            if (this.HasPlayer(name)) {
                this.RemovePlayer(name);
            }

            this[lane].Teams[teamIndex][position] = name;
        }

        public bool HasPlayer(string name) {
            if (string.IsNullOrEmpty(name)) {
                return false;
            }

            foreach (MatchData match in this) {
                foreach (TeamData team in match.Teams) {
                    if (team.Names.Contains(name)) {
                        return true;
                    }
                }
            }
            return false;
        }

        public PlayerLocation PollPlayer(string name) {
            foreach (MatchData match in this) {
                foreach (TeamData team in match.Teams) {
                    if (team.Names.Contains(name)) {
                        return new(match.Lane, Array.IndexOf(match.Teams, team), team.IndexOf(name));
                    }
                }
            }
            return new(-1, -1, -1);
        }

        public List<TeamData> Teams {
            get {
                List<TeamData> teams = [];

                foreach (MatchData match in this) {
                    foreach (TeamData team in match.Teams) {
                        teams.Add(team);
                    }
                }

                return teams;
            }
        }

        public IEnumerable<Record> Records() {
            foreach (MatchData matchData in this) {
                foreach (TeamData teamData in matchData.Teams) {
                    foreach (string name in teamData) {
                        yield return new Record(this, matchData, teamData, name);
                    }
                }
            }
        }

        public override string ToString() {
            StringBuilder sb = new();
            foreach (MatchData match in this) {
                sb.Append(match.ToString());
                sb.Append('\n');
            }
            return sb.ToString();
        }
    }
}
