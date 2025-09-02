using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Enums;
using System.Diagnostics;
using Utility.Extensions;

namespace Leagueinator.GUI.Controllers.Modules.ScoringPlus {

    public class PlusResult : IComparable<PlusResult>, IResult<PlusResult> {
        public PlusResult() {
            this.Opponents = "";
        }

        /// <summary>
        /// A single result for a player from a single match.
        /// </summary>
        public PlusResult(TeamData teamData) {
            this.TeamData = teamData;
            this.Lane = teamData.Parent.Lane;

            switch (teamData.Result) {
                case Model.Enums.GameResult.Vacant:
                    break;
                case Model.Enums.GameResult.Loss:
                    this.Score += (int)Constants.ScoringPolicy.LosePoints;
                    break;
                case Model.Enums.GameResult.Tie:
                    this.Score += (int)Constants.ScoringPolicy.DrawPoints;
                    break;
                case Model.Enums.GameResult.Win:
                    this.Score += (int)Constants.ScoringPolicy.WinPoints;
                    break;
            }

            this.Lane = teamData.Parent.Lane;
            this.TieBreaker = teamData.TieBreaker;
            this.Result = teamData.Result;
            this.ShotsFor += (int)Math.Min(teamData.ShotsFor, teamData.Parent.Ends * Constants.BowlsCapPerEnd);
            this.ShotsAgainst += (int)Math.Min(teamData.ShotsAgainst, teamData.Parent.Ends * Constants.BowlsCapPerEnd);
            this.PlusFor += teamData.ShotsFor - this.ShotsFor;
            this.PlusAgainst = teamData.ShotsAgainst - this.ShotsAgainst;
            this.Ends = teamData.Parent.Ends;
            this.Opponents = teamData.GetOpposition().SelectMany(t => t.ToPlayers()).JoinString();
        }

        public static string[] Labels => ["R", "DIFF", "PCT", "SF+", "SA+", "TB", "E", "Opponents"];

        public virtual string[] Cells() => [$"{this.Result.ToString()[0]}", $"{this.Diff}", $"{this.PCT:F1}", $"{this.ShotsFor}+{this.PlusFor}", $"{this.ShotsAgainst}+{this.PlusAgainst}", $"{this.TieBreaker.ToString()[0]}", $"{this.Ends}", $"{this.Opponents}"];

        public static int[] ColSizes => [40, 60, 60, 40, 40, 20, 40, 150];

        public TeamData TeamData { get; }
        public int Lane { get; set; } = -1;
        public int Score { get; set; } = 0;
        public GameResult Result { get; set; } = GameResult.Vacant;
        public int ShotsFor { get; set; } = 0;
        public int ShotsAgainst { get; set; } = 0;
        public int PlusFor { get; set; } = 0;
        public int PlusAgainst { get; set; } = 0;
        public int Ends { get; set; } = 0;
        public bool TieBreaker { get; }
        public int RawFor => this.ShotsFor + this.PlusFor;
        public int RawAgainst => this.ShotsAgainst + this.PlusAgainst;

        public int Diff => this.RawFor - this.RawAgainst;

        public double PCT {
            get {
                if (this.RawFor + this.RawAgainst == 0) return 0;
                return this.RawFor / ((double)this.RawFor + this.RawAgainst) * 100;
            }
        }

        public string Opponents { get; init; }

        public int CompareTo(PlusResult? other) {
            if (other is null) return -1; // this > null

            int c = other.Score.CompareTo(Score);  // higher Score first
            if (c != 0) return c;

            if (other.ShotsFor != this.ShotsFor) {
                return other.ShotsFor.CompareTo(this.ShotsFor);
            }

            if (other.PlusFor != this.PlusFor) {
                return other.PlusFor.CompareTo(this.PlusFor);
            }

            if (other.ShotsAgainst != this.ShotsAgainst) {
                return this.ShotsAgainst.CompareTo(this.ShotsAgainst);
            }

            if (other.PlusAgainst != this.PlusAgainst) {
                return this.PlusAgainst.CompareTo(this.PlusAgainst);
            }

            c = other.Diff.CompareTo(Diff);        // higher Differential first
            if (c != 0) return c;

            // higher Shot % first (with small tolerance)
            const double EPS = 1e-9;
            double delta = PCT - other.PCT;
            if (Math.Abs(delta) > EPS) return delta < 0 ? 1 : -1;

            return 0; // still tied → true tie
        }

        public static PlusResult CreateResult(TeamData teamData) {
            return new PlusResult(teamData);
        }

        public static PlusResult CreateResult() {
            return new PlusResult();
        }

        public static PlusResult operator +(PlusResult left, PlusResult right) {
            return new PlusResult {
                Ends = left.Ends + right.Ends,
                Score = left.Score + right.Score,
                ShotsFor = left.ShotsFor + right.ShotsFor,
                ShotsAgainst = left.ShotsAgainst + right.ShotsAgainst,
                PlusFor = left.PlusFor + right.PlusFor,
                PlusAgainst = left.PlusAgainst + right.PlusAgainst,
            };
        }
    }
}
