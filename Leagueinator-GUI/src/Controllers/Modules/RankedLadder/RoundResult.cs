using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Enums;

namespace Leagueinator.GUI.Controllers.Modules.RankedLadder {

    public static class RoundResultExtensions {
        public static RoundResult Sum(this List<RoundResult> results) {
            RoundResult sum = new();
            foreach (RoundResult result in results) {
                sum.Ends += result.Ends;
                sum.Score += result.Score;
                sum.BowlsFor += result.ShotsFor;
                sum.BowlsAgainst += result.BowlsAgainst;
                sum.PlusFor += result.PlusFor;
                sum.PlusAgainst += result.PlusAgainst;
            }            
            return sum;
        }
    }

    public record RoundResult : IComparable<RoundResult> {

        private List<string> _opponents = [];
        public List<string> Opponents {
            get => this._opponents;
            set => this._opponents = [.. value];
        }
        public int Lane { get; set; } = -1;
        public int Score { get; set; } = 0;
        public GameResult Result { get; set; } = GameResult.Vacant;
        public int BowlsFor { get; set; } = 0;
        public int BowlsAgainst { get; set; } = 0;
        public int PlusFor { get; set; } = 0;
        public int PlusAgainst { get; set; } = 0;
        public int ShotsFor => this.BowlsFor + this.PlusFor;
        public int ShotsAgainst => this.BowlsAgainst + this.PlusAgainst;

    public int Ends { get; set; } = 0;

        public int DiffBowls => this.BowlsFor - this.BowlsAgainst;

        public int DiffPlus => this.PlusFor - this.PlusAgainst;

        public double PCT => (double)this.ShotsFor / ((double)this.ShotsFor + this.ShotsAgainst);

        public RoundResult() { }

        public RoundResult(TeamData teamData) {
            this.Lane = teamData.Parent.Lane;

            switch (teamData.Result) {
                case Model.Enums.GameResult.Vacant:
                case Model.Enums.GameResult.Loss:
                    break;
                case Model.Enums.GameResult.Draw:
                    this.Score += 1;
                    break;
                case Model.Enums.GameResult.Win:
                    this.Score += 3;
                    break;
            }

            this.Ends = teamData.Parent.Ends;
            this.Result = teamData.Result;
            this.BowlsFor += (int)Math.Min(teamData.ShotsFor, this.Ends * 1.5);
            this.BowlsAgainst += (int)Math.Min(teamData.ShotsAgainst, this.Ends * 1.5);
            this.PlusFor = teamData.ShotsFor - this.BowlsFor;
            this.PlusAgainst = teamData.ShotsAgainst - this.BowlsAgainst;
        }

        public int CompareTo(RoundResult? other) {
            if (other is null) return -1; // this > null

            int c = other.Score.CompareTo(Score);  // higher Score first
            if (c != 0) return c;

            c = other.DiffBowls.CompareTo(DiffBowls);        // diff score w/o plus
            if (c != 0) return c;

            c = other.DiffPlus.CompareTo(DiffPlus);        // diff plus
            if (c != 0) return c;

            // higher Shot % first (with small tolerance)
            const double EPS = 1e-9;
            double delta = PCT - other.PCT;
            if (Math.Abs(delta) > EPS) return delta < 0 ? 1 : -1;

            return 0; // still tied → true tie
        }

        // Optional: convenience comparer instance
        public static readonly IComparer<RoundResult> OlbaComparer = Comparer<RoundResult>.Create((a, b) => a.CompareTo(b));
    }
}
