using Leagueinator.GUI.Controllers.NamedEvents;
using System.Windows;
using Leagueinator.Utility.Extensions;
using Leagueinator.GUI.Controls.MatchCards;

namespace Leagueinator.GUI.Forms.Main {
    public class MainWindowReceiver {

        private MainWindow MainWindow { get; set; }

        public MainWindowReceiver(MainWindow mainWindow) : base() {
            this.MainWindow = mainWindow;
        }

        [NamedEventHandler(EventName.FocusGranted)]
        internal void DoFocusGranted(int lane, int teamIndex) {
            TeamCard? card = this.MainWindow
                                 .GetDescendantsOfType<TeamCard>()
                                 .Where(card => card.MatchCard.Lane.Equals(lane))
                                 .FirstOrDefault(card => card.TeamIndex.Equals(teamIndex));

            if (card is not null) card.Background = AppColors.TeamPanelFocused;
        }

        [NamedEventHandler(EventName.FocusRevoked)]
        internal void DoFocusRevoked(int lane, int teamIndex) {
            TeamCard? card = this.MainWindow
                                .GetDescendantsOfType<TeamCard>()
                                .Where(card => card.MatchCard.Lane.Equals(lane))
                                .FirstOrDefault(card => card.TeamIndex.Equals(teamIndex));

            if (card is not null) card.Background = AppColors.TeamPanelDefault;
        }

        [NamedEventHandler(EventName.SetTitle)]
        internal void DoSetTitle(string title, bool saved) {

            if (saved) {
                this.MainWindow.Title = $"{title} [✔]";
            }
            else {
                this.MainWindow.Title = $"{title} [✘]";
            }
        }

        [NamedEventHandler(EventName.Notification)]
        internal void DoNotification(string message) {
            MessageBox.Show(message, "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
