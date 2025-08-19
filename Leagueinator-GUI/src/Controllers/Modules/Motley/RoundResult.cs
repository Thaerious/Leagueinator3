using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Enums;

namespace Leagueinator.GUI.Controllers.Modules.Motley {

    public static class RoundResultExtensions {
        public static RoundResult Sum(this List<RoundResult> results) {
            RoundResult sum = new();
            foreach (RoundResult result in results) {
                sum.GamesPlayed += result.GamesPlayed;
                sum.Score += result.Score;
                sum.Wins += result.Wins;
                sum.Draws += result.Draws;
                sum.Losses += result.Losses;
                sum.ShotsFor += result.ShotsFor;
                sum.ShotsAgainst += result.ShotsAgainst;
            }
            return sum;
        }
    }

    public record RoundResult : IComparable<RoundResult> {

        public int Lane { get; set; } = -1;        
        public int Score { get; set; } = 0;
        public GameResult Ressult { get; set; } = GameResult.Vacant;
        public int ShotsFor { get; set; } = 0;
        public int ShotsAgainst { get; set; } = 0;
        public int Ends { get; set; } = 0;

        public int Diff => this.ShotsFor - this.ShotsAgainst;

        public double PCT => (double)this.ShotsFor / ((double)this.ShotsFor + this.ShotsAgainst);

        public RoundResult() { }

        // Constructor that accepts EventData
        public RoundResult(RoundData roundData, string name) {
            this.Label = roundData.EventName;

            foreach (TeamData teamData in roundData.AllTeams()) {
                if (!teamData.Names.Contains(name)) continue;

                switch (teamData.Result) {
                    case Model.Enums.GameResult.Vacant:
                        break;
                    case Model.Enums.GameResult.Loss:
                        this.Score += 1;
                        this.Losses++;
                        break;
                    case Model.Enums.GameResult.Draw:
                        this.Score += 2;
                        this.Draws++;
                        break;
                    case Model.Enums.GameResult.Win:
                        this.Score += 3;
                        this.Wins++;
                        break;
                }

                this.ShotsFor += teamData.Shots;
                this.ShotsAgainst += teamData.ShotsAgainst;
                this.GamesPlayed++;
            }
        }

        public int CompareTo(RoundResult? other) {
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

        // Optional: convenience comparer instance
        public static readonly IComparer<RoundResult> OlbaComparer = Comparer<RoundResult>.Create((a, b) => a.CompareTo(b));
    }
}
