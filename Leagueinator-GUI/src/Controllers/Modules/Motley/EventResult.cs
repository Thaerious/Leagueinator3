using Leagueinator.GUI.Model;

namespace Leagueinator.GUI.Controllers.Modules.Motley {

    public static class EventResultExtensions {
        public static EventResult Sum(this List<EventResult> results) {
            EventResult sum = new();
            foreach (EventResult result in results) {
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

    public record EventResult : IComparable<EventResult> {

        public string Label { get; set; } = "";
        
        public int Score { get; set; } = 0;
        public int Wins { get; set; } = 0;
        public int Draws { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public int ShotsFor { get; set; } = 0;
        public int ShotsAgainst { get; set; } = 0;
        public int GamesPlayed { get; set; } = 0;

        public int Diff => this.ShotsFor - this.ShotsAgainst;

        public double PCT => (double)this.ShotsFor / ((double)this.ShotsFor + this.ShotsAgainst);

        public EventResult() { }

        // Constructor that accepts EventData
        public EventResult(EventData eventData, string name) {
            this.Label = eventData.EventName;

            foreach (TeamData teamData in eventData.AllTeams()) {
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

                this.ShotsFor += teamData.ShotsFor;
                this.ShotsAgainst += teamData.ShotsAgainst;
                this.GamesPlayed++;
            }
        }

        public int CompareTo(EventResult? other) {
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
        public static readonly IComparer<EventResult> OlbaComparer = Comparer<EventResult>.Create((a, b) => a.CompareTo(b));
    }
}
