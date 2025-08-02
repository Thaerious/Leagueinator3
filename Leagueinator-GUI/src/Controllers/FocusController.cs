using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Controllers.NamedEvents;
using Utility.Extensions;

namespace Leagueinator.GUI.src.Controllers {
    public class FocusController {
        public MainController MainController { get; }

        private List<TeamID> Focused = [];

        public FocusController(MainController mainController) {
            this.MainController = mainController;
        }

        public record FocusArgs(TeamID TeamId, string Action);

        public delegate void FocusGranted(object sender, FocusArgs args);
        public delegate void FocusRevoked(object sender, FocusArgs args);

        public event FocusGranted OnFocusGranted = delegate { };
        public event FocusRevoked OnFocusRevoked = delegate { };

        private void InvokeFocusGranted(TeamID teamID) {
            this.DispatchEvent(EventName.FocusGranted, new () {
                ["teamIndex"] = teamID.TeamIndex,
                ["lane"] = teamID.Lane
            });
        }

        private void InvokeFocusRevoked(TeamID teamID) {
            this.DispatchEvent(EventName.FocusRevoked, new() {
                ["teamIndex"] = teamID.TeamIndex,
                ["lane"] = teamID.Lane
            });
        }

        [NamedEventHandler(EventName.RequestFocus)]
        internal void DoRequestFocus(int lane, int teamIndex, bool append) {
            TeamID teamID = new(teamIndex, lane);

            if (append) {
                if (this.Focused.Remove(teamID)) {                    
                    this.InvokeFocusRevoked(teamID);
                }
                else {
                    this.Focused.Add(teamID);
                    this.InvokeFocusGranted(teamID);
                }
            }
            else {
                if (this.Focused.Contains(teamID)) return;
                this.ClearFocus();
                this.InvokeFocusGranted(teamID);
                this.Focused.Add(teamID);
            }
        }

        [NamedEventHandler(EventName.SwapTeams)]
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
                this.MainController.DispatchRoundUpdated(EventName.RoundChanged);

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
