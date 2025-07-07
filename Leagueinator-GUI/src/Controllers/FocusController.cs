using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Utility.Extensions;
using System.Diagnostics;

namespace Leagueinator.GUI.src.Controllers {
    public class FocusController {
        public MainController MainController { get; }
        public NamedEventReceiver NamedEventRcv { get; private set; }
        public NamedEventDispatcher NamedEventDisp { get; set; }

        private List<TeamID> Focused = [];

        public FocusController(MainController mainController) {
            this.MainController = mainController;
            this.NamedEventRcv = new(this);
            this.NamedEventDisp = new(this);
        }

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

            if (append) {
                if (this.Focused.Contains(teamId)) {
                    this.Focused.Remove(teamId);
                    this.InvokeFocusRevoked(teamId);
                }
                else {
                    this.Focused.Add(teamId);
                    this.InvokeFocusGranted(teamId);
                }
            }
            else {
                this.ClearFocus();
                this.InvokeFocusGranted(teamId);
                this.Focused.Add(teamId);
            }
        }

        internal void DoSwap() {
            if (this.Focused.Count <= 1) return;
            List<TeamID> swapList = [.. this.Focused];

            TeamID from = swapList.Pop();

            while(swapList.Count > 0){
                TeamID to = swapList.Pop();

                var tempFrom = this.MainController.RoundData[from.Lane].Teams[from.TeamIndex];
                var tempTo = this.MainController.RoundData[to.Lane].Teams[to.TeamIndex];
                this.MainController.RoundData[from.Lane].Teams[from.TeamIndex] = tempTo;
                this.MainController.RoundData[to.Lane].Teams[to.TeamIndex] = tempFrom;
                this.MainController.InvokeRoundUpdate();

                from = to;
            }
        }

        private void ClearFocus() {
            foreach (var teamId in this.Focused) {
                this.InvokeFocusRevoked(teamId);
            }
            this.Focused = [];
        }
    }
}
