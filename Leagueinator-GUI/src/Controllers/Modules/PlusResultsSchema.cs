using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Enums;
using Leagueinator.GUI.Model.Results.BowlsPlus;
using Leagueinator.GUI.Model.ViewModel;

namespace Leagueinator.GUI.Controllers.Modules {
    public class PlusResultsSchema : ResultSchema, IComparable<PlusResultsSchema> {

        public PlusResultsSchema() { }

        public PlusResultsSchema(TeamData teamData) {
            int maxBowls = (int)Math.Floor(teamData.Parent.Ends * 1.5);
            int sumAgainst = teamData.Parent.Teams.Where(t => t != teamData).Select(t => t.ShotsFor).Sum();

            this.Lane = teamData.Parent.Lane;
            this.Ends = teamData.Parent.Ends;

            this.Opponents = teamData.Parent.Teams
                                     .Where(t => t != teamData)
                                     .SelectMany(t => t.Players)
                                     .ToList();

            this.Result = teamData.Result;
            this.BowlsFor = Math.Min(teamData.ShotsFor, maxBowls);
            this.BowlsAgainst = Math.Min(sumAgainst, maxBowls);
            this.PlusFor = teamData.ShotsFor - this.BowlsFor;
            this.PlusAgainst = sumAgainst - this.BowlsAgainst;

            if (teamData.Result == GameResult.Win) this.Wins = 1;
        }

        public static PlusResultsSchema operator +(PlusResultsSchema left, PlusResultsSchema right) {
            return new PlusResultsSchema() {
                BowlsFor = left.BowlsFor + right.BowlsFor,
                BowlsAgainst = left.BowlsAgainst + right.BowlsAgainst,
                PlusFor = left.PlusFor + right.PlusFor,
                PlusAgainst = left.PlusAgainst + right.PlusAgainst,
                Wins = left.Wins + right.Wins
            };
        }

        public int? Lane { get; set; }
        public int? Ends { get; set; }

        public List<string> Opponents { get; set; } = [];

        public GameResult? Result { get; }

        public int Wins { get; private set; }

        public int BowlsFor { get; init; }
        public int BowlsAgainst { get; init; }
        public int PlusFor { get; init; }
        public int PlusAgainst { get; init; }

        public override List<string> Fields => new() {
            "Result", "BFor", "BAgainst", "PFor", "PAgainst", "TB"
        };

        public override List<object?> Values => [
            this.Result, this.BowlsFor, this.BowlsAgainst, this.PlusFor, this.PlusAgainst
        ];

        public int CompareTo(PlusResultsSchema? other) {
            if (other == null) return 1; // Null is less

            int cmp = this.Wins.CompareTo(other.Wins);
            if (cmp != 0) return cmp; // Higher is better

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
    }
}
