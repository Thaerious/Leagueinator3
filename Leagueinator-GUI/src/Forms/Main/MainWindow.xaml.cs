using Leagueinator.GUI.Controllers.NamedEvents;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Leagueinator.GUI.Forms.Main {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private bool AllowClose = false;

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

        protected override void OnClosing(CancelEventArgs args) {
            Debug.WriteLine($"OnClosing({this.AllowClose})");
            if (this.AllowClose) { 
                base.OnClosing(args); 
                return; 
            }

            args.Cancel = true;
            Dispatcher.BeginInvoke(new Action(() => this.DispatchEvent(EventName.Terminate)));
        }

        public void ApproveClose() {
            Debug.WriteLine("ApproveClose");
            this.AllowClose = true;
            Close();
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
        internal static void DoNotification(string message, AlertLevel alertLevel) {
            var image = MessageBoxImage.Information;
            string title = "Information";

            switch (alertLevel) {
                case AlertLevel.Inform:
                    image = MessageBoxImage.Information;
                    title = "Information";
                    break;
                case AlertLevel.Warning:
                    image = MessageBoxImage.Exclamation;
                    title = "Warning";
                    break;
                case AlertLevel.Error:
                    image = MessageBoxImage.Error;
                    title = "Error";
                    break;
            }


            MessageBox.Show(message, title, MessageBoxButton.OK, image);
        }

        [NamedEventHandler(EventName.ConfirmExit)]
        internal void DoConfirmExit() {
            var result = MessageBox.Show(
                "League not saved, confirm exit?",
                "Confirm Exit",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes) {
                this.ApproveClose();
            }
        }

        [NamedEventHandler(EventName.ConfirmLoad)]
        internal void DoConfirmSave() {
            var result = MessageBox.Show(
                "League not saved, confirm load league?",
                "Confirm Load",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes) {
                this.DispatchEvent(EventName.LoadConfirmed);
            }
        }

        [NamedEventHandler(EventName.ConfirmNew)]
        internal void DoConfirmNew() {
            var result = MessageBox.Show(
                "League not saved, confirm new league?",
                "Confirm New",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes) {
                this.DispatchEvent(EventName.NewConfirmed);
            }
        }
    }
}
