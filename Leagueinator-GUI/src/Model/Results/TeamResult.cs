using System.Diagnostics;
using System.Text;

namespace Leagueinator.GUI.Model.Results {
    internal class TeamResult : IComparable<TeamResult>{
        public readonly List<SingleResult> MatchResults = [];

        public TeamData Team {
            get; private set;
        }

        public TeamResult(TeamData team) {
            this.Team = team;
        }

        public int Wins {
            get {
                if (this.MatchResults.Count == 0) return 0;
                return this.MatchResults.Count(r => r.Result == Result.Win);
            }
        }

        public int Draws {
            get {
                if (this.MatchResults.Count == 0) return 0;
                return this.MatchResults.Count(r => r.Result == Result.Draw);
            }
        }

        public int Losses {
            get {
                if (this.MatchResults.Count == 0) return 0;
                return this.MatchResults.Count(r => r.Result == Result.Loss);
            }
        }

        public int Games {
            get {
                return this.MatchResults.Count;
            }
        }

        public int Ends {
            get {
                if (this.MatchResults.Count == 0) return 0;
                return this.MatchResults.Sum(r => r.Ends);
            }
        }

        public int Bowls {
            get {
                if (this.MatchResults.Count == 0) return 0;
                return this.MatchResults.Sum(r => r.Bowls);
            }
        }

        public int Against {
            get {
                if (this.MatchResults.Count == 0) return 0;
                return this.MatchResults.Sum(r => r.Against);
            }
        }

        public int Rank { get; set; } = -1;

        public int CountWins {
            get {
                if (this.MatchResults.Count == 0) return 0;
                return this.MatchResults.Count(r => r.Result == Result.Win);
            }
        }

        public int CountEnds {
            get {
                if (this.MatchResults.Count == 0) return 0;
                return this.MatchResults.Sum(r => r.Ends);
            }
        }

        public int BowlsFor {
            get {
                if (this.MatchResults.Count == 0) return 0;
                return this.MatchResults.Sum(r => r.BowlsFor);
            }
        }

        public int BowlsAgainst {
            get {
                if (this.MatchResults.Count == 0) return 0;
                return this.MatchResults.Sum(r => r.BowlsAgainst);
            }
        }

        public int PlusFor {
            get {
                if (this.MatchResults.Count == 0) return 0;
                return this.MatchResults.Sum(r => r.PlusFor);
            }
        }

        public int PlusAgainst {
            get {
                if (this.MatchResults.Count == 0) return 0;
                return this.MatchResults.Sum(r => r.PlusAgainst);
            }
        }
        public int CompareTo(TeamResult? other) {
            if (other == null) return 1; // Null is less

            int cmp = this.Wins.CompareTo(other.Wins);
            if (cmp != 0) return cmp;

            cmp = this.Draws.CompareTo(other.Draws);
            if (cmp != 0) return cmp;

            cmp = this.BowlsFor.CompareTo(other.BowlsFor); // Higher is better
            if (cmp != 0) return cmp;

            cmp = this.PlusFor.CompareTo(other.PlusFor); // Higher is better
            if (cmp != 0) return cmp;

            cmp = other.BowlsAgainst.CompareTo(this.BowlsAgainst); // Lower is better
            if (cmp != 0) return cmp;

            cmp = other.PlusAgainst.CompareTo(this.PlusAgainst); // Lower is better
            if (cmp != 0) return cmp;

            return 0; // Equal results
        }

        public override string ToString() {
            return $"{this.Rank}: {string.Join(", ", this.Team)} #{this.MatchResults.Count}  (Wins: {this.Wins}, Draws: {this.Draws}, Losses: {this.Losses}, For: {this.BowlsFor}+{this.PlusFor}, Against: {this.BowlsAgainst}+{this.PlusAgainst}, Ends: {this.Ends})";
        }
    }
}
