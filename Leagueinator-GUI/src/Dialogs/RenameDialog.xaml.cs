using System.Windows;
using System.Windows.Input;

namespace Leagueinator.GUI.Dialogs {
    /// <summary>
    /// Interaction logic for RenameDialog.xaml
    /// </summary>
    public partial class RenameDialog : Window {
        public string NewName { get; private set; } = "";

        public RenameDialog(string currentName) {
            this.InitializeComponent();
            this.txtNewName.Text = currentName;
            this.txtNewName.SelectAll();
            this.txtNewName.Focus();

            this.txtNewName.KeyDown += this.HndKeyDown;
        }

        private void HndKeyDown(object sender, System.Windows.RoutedEventArgs e) {
            if (e is not KeyEventArgs keyArgs) return;

            if (keyArgs.Key == Key.Enter) {
                this.NewName = this.txtNewName.Text;
                this.DialogResult = true;
                this.Close();
            }
        }

        private void HndOkButtonClick(object sender, RoutedEventArgs e) {
            // Set the NewName property To the text in the textbox.
            this.NewName = this.txtNewName.Text;
            this.DialogResult = true;
            this.Close();
        }

        private void HndCancelButtonClick(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
            this.Close();
        }
    }
}
