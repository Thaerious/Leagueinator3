using Leagueinator.GUI.Controllers.NamedEvents;
using System.Windows;
using System.Windows.Input;

namespace Leagueinator.GUI.Forms.Main {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            NamedEvent.RegisterHandler(this, true);
            this.InitializeComponent();
            this.Title = "Leagueinator []";
            this.Loaded += this.OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            this.RoundPanel.PreviewMouseDown += (s, e) => {
                Keyboard.ClearFocus();
            };
            this.MatchPanel.PreviewMouseDown += (s, e) => {
                Keyboard.ClearFocus();
            };
            this.ResumeEvents();
        }

        public void ClearFocus() {
            FocusManager.SetFocusedElement(this, null);
        }

        [NamedEventHandler(EventName.SetTitle)]
        internal void DoSetTitle(string title, bool saved) {
            if (saved) {
                this.Title = $"{title} [✔]";
            }
            else {
                this.Title = $"{title} [✘]";
            }
        }

        [NamedEventHandler(EventName.Notification)]
        internal static void DoNotification(string message) {
            MessageBox.Show(message, "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
