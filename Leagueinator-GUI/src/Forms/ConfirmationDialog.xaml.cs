using System.Windows;

namespace Leagueinator.GUI.Forms {

    public partial class ConfirmationDialog : Window {

        public ConfirmationDialog() {
            InitializeComponent();
        }

        public string Text {
            set => this.DisplayText.Text = value;
            get => this.DisplayText.Text;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = true;
        }

        private void NoButton_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
        }
    }
}
