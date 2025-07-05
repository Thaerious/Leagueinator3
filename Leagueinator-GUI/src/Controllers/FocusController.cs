using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Forms.Main;
using System.Diagnostics;
using System.Windows;

namespace Leagueinator.GUI.src.Controllers {
    public class FocusController(MainController mainController) : NamedEventController {
        public MainController MainController { get; } = mainController;

        private List<TeamID> Focused = [];

        public record FocusArgs(TeamID TeamId, string Action);

        public delegate void FocusGranted(object sender, FocusArgs args);
        public delegate void FocusRevoked(object sender, FocusArgs args);

        public event FocusGranted OnFocusGranted = delegate { };
        public event FocusRevoked OnFocusRevoked = delegate { };

        private void InvokeFocusGranted(TeamID target) {
            FocusArgs args = new(target, "granted");
            this.OnFocusGranted.Invoke(this, args);
        }

        private void InvokeFocusRevoked(TeamID target) {
            Debug.WriteLine("InvokeFocusRevoked: " + target);
            FocusArgs args = new(target, "revoked");
            this.OnFocusRevoked.Invoke(this, args);
        }

        internal void DoRequestFocus(int lane, int teamIndex, bool append) {
            TeamID teamId = new(teamIndex, lane);

            if (append == false) this.ClearFocus();
            if (this.Focused.Contains(teamId)) return;
            this.InvokeFocusGranted(teamId);
            this.Focused.Add(teamId);
        }

        internal void DoClearFocus(NamedEventArgs e) { }

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

        private void DoSwap(TeamID from, TeamID to) {
            Debug.WriteLine($"Swapping {from} with {to}");
            var tempFrom = this.MainController.RoundData[from.Lane].Teams[from.TeamIndex];
            var tempTo = this.MainController.RoundData[to.Lane].Teams[to.TeamIndex];
            this.MainController.RoundData[from.Lane].Teams[from.TeamIndex] = tempTo;
            this.MainController.RoundData[to.Lane].Teams[to.TeamIndex] = tempFrom;
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
