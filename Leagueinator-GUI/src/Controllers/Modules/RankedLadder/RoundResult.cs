using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Enums;
using Utility;
using Utility.Collections;

namespace Leagueinator.GUI.Controllers.Modules.RankedLadder {

    /// <summary>
    /// Extensions for aggregating <see cref="RoundResult"/> values.
    /// </summary>
    public static class RoundResultExtensions {
        /// <summary>
        /// Sums per-field tallies across a list of <see cref="RoundResult"/>.
        /// Assumes results is non-null; returns a fresh <see cref="RoundResult"/>.
        /// </summary>
        public static RoundResult Sum(this List<RoundResult> results) {
            RoundResult sum = new();
            foreach (RoundResult result in results) {
                if (result is null) continue;
                sum.Ends += result.Ends;
                sum.Score += result.Score;
                sum.BowlsFor += result.BowlsFor;
                sum.BowlsAgainst += result.BowlsAgainst;
                sum.PlusFor += result.PlusFor;
                sum.PlusAgainst += result.PlusAgainst;
            }
            return sum;
        }
    }

    /// <summary>
    /// Per-round scoring/metrics for a team, comparable for leaderboard sorting.
    /// Higher is better by: Score, DiffBowls, DiffPlus, then Shot% (PCT).
    /// </summary>
    public class RoundResult : IComparable<RoundResult> {

        // Backing store so the setter can defensively copy.
        private List<string> _opponents = [];
        /// <summary>
        /// Opponents' names for this round. Order not specified.
        /// Setter copies input to avoid external list aliasing.
        /// </summary>
        public List<string> Opponents {
            get => [.. this._opponents];
            set => this._opponents = [.. value];
        }

        /// <summary>Lane number; -1 when unset.</summary>
        public int Lane { get; set; } = -1;

        /// <summary>Match points for this round (e.g., Win=3, Draw=1, Loss/Vacant=0).</summary>
        public int Score { get; set; } = 0;

        /// <summary>W/D/L/Vacant result.</summary>
        public GameResult Result { get; set; } = GameResult.Vacant;

        /// <summary>“Bowls” component of shots-for (clamped to Ends * 1.5).</summary>
        public int BowlsFor { get; set; } = 0;

        /// <summary>“Bowls” component of shots-against (clamped to Ends * 1.5).</summary>
        public int BowlsAgainst { get; set; } = 0;

        /// <summary>“Plus” (overflow above bowls cap) component for.</summary>
        public int PlusFor { get; set; } = 0;

        /// <summary>“Plus” (overflow above bowls cap) component against.</summary>
        public int PlusAgainst { get; set; } = 0;

        /// <summary>Total shots-for (BowlsFor + PlusFor).</summary>
        public int ShotsFor => this.BowlsFor + this.PlusFor;

        /// <summary>Total shots-against (BowlsAgainst + PlusAgainst).</summary>
        public int ShotsAgainst => this.BowlsAgainst + this.PlusAgainst;

        /// <summary>True if this team won by tiebreaker.</summary>
        public bool TieBreaker { get; set; } = false;

        /// <summary>Ends played in this round.</summary>
        public int Ends { get; set; } = 0;

        /// <summary>Bowls differential (for - against), excluding “Plus”.</summary>
        public int DiffBowls => this.BowlsFor - this.BowlsAgainst;

        /// <summary>Plus differential (for - against).</summary>
        public int DiffPlus => this.PlusFor - this.PlusAgainst;

        /// <summary>
        /// Shot percentage = ShotsFor / (ShotsFor + ShotsAgainst).
        /// NOTE: No zero-denominator guard here; caller must ensure denominator &gt; 0.
        /// </summary>
        public double PCT {
            get {
                if (this.ShotsFor + this.ShotsAgainst == 0) return 0.0;
                return (double) this.ShotsFor / ((double)this.ShotsFor + this.ShotsAgainst);
            }
        }

        /// <summary>Default constructor for accumulation/empty initialization.</summary>
        public RoundResult() { }

        /// <summary>
        /// Build a <see cref="RoundResult"/> from a team’s round data.
        /// - Translates <see cref="GameResult"/> to points (Win=3, Draw=1, Loss/Vacant=0).
        /// - Clamps bowls components to (Ends * 1.5), overflow goes into “Plus”.
        /// </summary>
        public RoundResult(TeamData teamData) {
            this.Lane = teamData.Parent.Lane;

            // Translate result → points.
            switch (teamData.Result) {
                case Model.Enums.GameResult.Vacant:
                case Model.Enums.GameResult.Loss:
                    this.Score += (int)Constants.ScoringPolicy.LosePoints;
                    break;
                case Model.Enums.GameResult.Draw:
                    this.Score += (int)Constants.ScoringPolicy.DrawPoints;
                    break;
                case Model.Enums.GameResult.Win:
                    this.Score += (int)Constants.ScoringPolicy.WinPoints;
                    break;
            }

            this.TieBreaker = teamData.TieBreaker;
            this.Ends = teamData.Parent.Ends;
            this.Result = teamData.Result;

            // Clamp bowls to Ends * 1.5; remainder goes to Plus.
            // NOTE: Math.Min(double, double) then cast to int → truncation toward zero.
            this.BowlsFor += (int)Math.Min(teamData.ShotsFor, this.Ends * Constants.BowlsCapPerEnd);
            this.BowlsAgainst += (int)Math.Min(teamData.ShotsAgainst, this.Ends * Constants.BowlsCapPerEnd);
            this.PlusFor = teamData.ShotsFor - this.BowlsFor;
            this.PlusAgainst = teamData.ShotsAgainst - this.BowlsAgainst;
        }

        /// <summary>
        /// Descending ordering by: Score, DiffBowls, DiffPlus, then Shot% with an epsilon.
        /// Returns &lt;0 if this should come before <paramref name="other"/> in the ranking.
        /// </summary>
        public int CompareTo(RoundResult? other) {
            if (other is null) return 1; 

            // Higher Score first.
            int c = other.Score.CompareTo(Score);
            if (c != 0) return c;

            // Higher bowls differential first (excluding Plus).
            c = other.DiffBowls.CompareTo(DiffBowls);
            if (c != 0) return c;

            // Higher plus differential next.
            c = other.DiffPlus.CompareTo(DiffPlus);
            if (c != 0) return c;

            // Higher Shot% first (with small tolerance to avoid jitter on ties).
            const double EPS = 1e-9;
            double delta = PCT - other.PCT;
            if (Math.Abs(delta) > EPS) return delta < 0 ? 1 : -1;

            // Still tied → true tie.
            return 0;
        }

        /// <summary>
        /// Computes per-team round results and their sums for an event, sorted by <see cref="RoundResult.CompareTo"/>.
        /// Empty teams are skipped.
        /// </summary>
        public static List<(TeamData Team, List<RoundResult> List, RoundResult Sum)> EventScores(EventData eventData) {
            // DefaultDictionary creates the List<RoundResult> on first access.
            DefaultDictionary<TeamData, List<RoundResult>> dictionary = new((_) => []);

            foreach (TeamData teamData in eventData.AllTeams()) {
                if (teamData.IsEmpty()) continue;

                RoundResult rr = new(teamData) {
                    // Snapshot opponents’ names now; order not guaranteed.
                    Opponents = [.. teamData.GetOpposition().AllNames()]
                };

                dictionary[teamData].Add(rr);
            }

            // OrderBy uses RoundResult.CompareTo for Sum (descending by our rules).
            return dictionary
                .Select(kvp => (Team: kvp.Key, List: kvp.Value, Sum: kvp.Value.Sum()))
                .OrderBy(tuple => tuple.Sum)
                .ToList();
        }

        /// <summary>Convenience comparer using the same CompareTo semantics.</summary>
        public static readonly IComparer<RoundResult> OlbaComparer =
            Comparer<RoundResult>.Create((a, b) => a.CompareTo(b));
    }
}
