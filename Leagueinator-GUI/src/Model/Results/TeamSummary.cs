namespace Leagueinator.GUI.Model.Results {

    /// <summary>
    /// A collection of all team results for a single set of players in an event.
    /// Used to report a team's overall score for the event.
    /// </summary>
    public class TeamSummary : List<TeamResults>, IComparable<TeamSummary> {
        public readonly Players Players;

        public TeamSummary(IEnumerable<string> players) {
            this.Players = new(players);
        }

        public int CompareTo(TeamSummary? other) {
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


        public int Wins {
            get {
                if (this.Count == 0) return 0;
                return this.Count(r => r.Result == Result.Win);
            }
        }

        public int Draws {
            get {
                if (this.Count == 0) return 0;
                return this.Count(r => r.Result == Result.Draw);
            }
        }

        public int Losses {
            get {
                if (this.Count == 0) return 0;
                return this.Count(r => r.Result == Result.Loss);
            }
        }

        public int Games {
            get {
                return this.Count;
            }
        }

        public int Ends {
            get {
                if (this.Count == 0) return 0;
                return this.Sum(r => r.Ends);
            }
        }

        public int Bowls {
            get {
                if (this.Count == 0) return 0;
                return this.Sum(r => r.Bowls);
            }
        }

        public int Against {
            get {
                if (this.Count == 0) return 0;
                return this.Sum(r => r.Against);
            }
        }

        public int Rank { get; set; } = -1;

        public int CountWins {
            get {
                if (this.Count == 0) return 0;
                return this.Count(r => r.Result == Result.Win);
            }
        }

        public int CountEnds {
            get {
                if (this.Count == 0) return 0;
                return this.Sum(r => r.Ends);
            }
        }

        public int BowlsFor {
            get {
                if (this.Count == 0) return 0;
                return this.Sum(r => r.BowlsFor);
            }
        }

        public int BowlsAgainst {
            get {
                if (this.Count == 0) return 0;
                return this.Sum(r => r.BowlsAgainst);
            }
        }

        public int PlusFor {
            get {
                if (this.Count == 0) return 0;
                return this.Sum(r => r.PlusFor);
            }
        }

        public int PlusAgainst {
            get {
                if (this.Count == 0) return 0;
                return this.Sum(r => r.PlusAgainst);
            }
        }

        public override string ToString() {
            return $"{this.Rank}: {string.Join(", ", this.Players)} #{this.Count}  (Wins: {this.Wins}, Draws: {this.Draws}, Losses: {this.Losses}, For: {this.BowlsFor}+{this.PlusFor}, Against: {this.BowlsAgainst}+{this.PlusAgainst}, Ends: {this.Ends})";
        }
    }
}
