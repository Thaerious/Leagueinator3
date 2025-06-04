using System.Windows;

namespace Leagueinator.GUI.Controls {
    public class MatchCardNameChangedArgs : RoutedEventArgs {
        public int TeamIndex { get; }
        public int LaneIndex { get; }
        public string OldName { get; }
        public string NewName { get; }

        public MatchCardNameChangedArgs(
            RoutedEvent routedEvent,
            object source,
            int teamIndex,
            int laneIndex,
            string oldName,
            string newName
        ) : base(routedEvent, source) {
            this.TeamIndex = teamIndex;
            this.LaneIndex = laneIndex;
            this.OldName = oldName;
            this.NewName = newName;
        }
    }
}
