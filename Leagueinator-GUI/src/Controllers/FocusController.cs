using Leagueinator.GUI.Controllers;
using System.Diagnostics;
using System.Windows;
using static Leagueinator.GUI.Forms.Main.MainWindow;

namespace Leagueinator.GUI.src.Controllers {
    public class FocusController(MainController mainController) {
        public MainController MainController { get; } = mainController;

        private List<TeamID> Focused = [];

        public record FocusArgs(TeamID TeamId, string Action);

        public delegate void FocusGranted(object sender, FocusArgs args);
        public delegate void FocusRevoked(object sender, FocusArgs args);

        public event FocusGranted OnFocusGranted = delegate { };
        public event FocusRevoked OnFocusRevoked = delegate { };

        public void InvokeFocusGranted(TeamID target) {
            FocusArgs args = new(target, "granted");
            this.OnFocusGranted.Invoke(this, args);
        }

        public void InvokeFocusRevoked(TeamID target) {
            Debug.WriteLine("InvokeFocusRevoked: " + target);
            FocusArgs args = new(target, "revoked");
            this.OnFocusRevoked.Invoke(this, args);
        }

        public void RequestFocusHnd(object sender, RoutedEventArgs e) {
            if (e is RequestFocusArgs args) {
                if (args.Append == false) this.ClearFocus();
                if (this.Focused.Contains(args.Target)) return;
                this.InvokeFocusGranted(args.Target);
                this.Focused.Add(args.Target);
            }
            else if (e is ClearFocusArgs) {                
                this.ClearFocus();
            }
            else {
                throw new NotSupportedException($"Unsupported event type: {e.GetType()}");
            }
        }

        public void RoundDataHnd(object? sender, RoundDataEventArgs e) {
            switch (e.Action) {
                case "Swap": {
                        if (this.Focused.Count <= 1) return;

                        for(int i = 0; i < this.Focused.Count - 1; i++) {
                            this.DoSwap(this.Focused[i], this.Focused[i+1]);
                        }

                        this.ClearFocus();
                    }
                    break;
            }
        }

        private void DoSwap(TeamID from, TeamID to) {
            Debug.WriteLine($"Swapping {from} with {to}");
            var tempFrom = this.MainController.RoundData[from.MatchIndex].Teams[from.TeamIndex];
            var tempTo = this.MainController.RoundData[to.MatchIndex].Teams[to.TeamIndex];
            this.MainController.RoundData[from.MatchIndex].Teams[from.TeamIndex] = tempTo;
            this.MainController.RoundData[to.MatchIndex].Teams[to.TeamIndex] = tempFrom;
            this.MainController.InvokeRoundEvent("Update");
        }

        private void ClearFocus() {
            foreach (var teamId in this.Focused) {
                this.InvokeFocusRevoked(teamId);
            }
            this.Focused = [];
        }
    }
}
