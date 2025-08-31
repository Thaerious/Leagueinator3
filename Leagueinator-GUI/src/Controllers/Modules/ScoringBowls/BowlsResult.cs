using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Enums;
using Utility.Extensions;

namespace Leagueinator.GUI.Controllers.Modules.ScoringBowls {

    public class BowlsResult : IComparable<BowlsResult>, IResult<BowlsResult> {
        public BowlsResult() {
            this.Opponents = "";
        }

        /// <summary>
        /// A single result for a player from a single match.
        /// </summary>
        /// <param name="forPlayer"></param>
        /// <param name="teamData"></param>
        public BowlsResult(TeamData teamData) {
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

            this.TieBreaker = teamData.TieBreaker;
            this.Result = teamData.Result;
            this.ShotsFor += teamData.ShotsFor;
            this.ShotsAgainst += teamData.ShotsAgainst;
            this.Ends = teamData.Parent.Ends;
            this.Opponents = teamData.GetOpposition().SelectMany(t => t.ToPlayers()).JoinString();
        }

        public static string[] Labels => ["PTS", "SF", "SA", "TB", "Ends", "Opponents"];
        public virtual string[] Cells() => [$"{this.Score}", $"{this.ShotsFor}", $"{this.ShotsAgainst}", $"{this.TieBreaker}", $"{this.Ends}", $"{this.Opponents.JoinString()}"];

        public static int[] ColSizes => [10, 10, 10, 10, 10, 100];

        public int Lane { get; set; } = -1;
        public int Score { get; set; } = 0;
        public GameResult Result { get; set; } = GameResult.Vacant;
        public int ShotsFor { get; set; } = 0;
        public int ShotsAgainst { get; set; } = 0;
        public int Ends { get; set; } = 0;
        public bool TieBreaker { get; }

        public int Diff => this.ShotsFor - this.ShotsAgainst;

        public double PCT => (double)this.ShotsFor / ((double)this.ShotsFor + this.ShotsAgainst);

        public string Opponents { get; init; } // TODO make this a list

        public int CompareTo(BowlsResult? other) {
            if (other is null) return -1; // this > null

            int c = other.Score.CompareTo(Score);  // higher Score first
            if (c != 0) return c;

            c = other.Diff.CompareTo(Diff);        // higher Differential first
            if (c != 0) return c;

            // higher Shot % first (with small tolerance)
            const double EPS = 1e-9;
            double delta = PCT - other.PCT;
            if (Math.Abs(delta) > EPS) return delta < 0 ? 1 : -1;

            return 0; // still tied → true tie
        }

        public static BowlsResult CreateResult(TeamData teamData) {
            return new BowlsResult(teamData);
        }

        public static BowlsResult CreateResult() {
            return new BowlsResult();
        }

        public static BowlsResult operator +(BowlsResult left, BowlsResult right) {
            return new BowlsResult {
                Ends = left.Ends + right.Ends,
                Score = left.Score + right.Score,
                ShotsFor = left.ShotsFor + right.ShotsFor,
                ShotsAgainst = left.ShotsAgainst + right.ShotsAgainst
            };
        }
    }
}
