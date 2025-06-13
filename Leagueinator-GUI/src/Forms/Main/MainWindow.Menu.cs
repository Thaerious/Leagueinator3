using Leagueinator.GUI.Dialogs;
using Leagueinator.GUI.Forms.Print;
using Leagueinator.GUI.Model;
using Microsoft.Win32;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Leagueinator.GUI.Forms.Main {

    [System.Runtime.Versioning.SupportedOSPlatform("windows")] // get rid of CA1416 warning
    public partial class MainWindow : Window {

        public event EventHandler<EventData>? OnEventData;

        public void HndMenuClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
        }

        public void HndSettings(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            SettingsDialog settingsDialog = new(this.EventData) {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (settingsDialog.ShowDialog() == true) {
                this.EventData = settingsDialog.EventData;
                this.OnEventData?.Invoke(this, settingsDialog.EventData);
            }
        }

        /// <summary>
        /// Triggered when the delete round button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HndDeleteRnd(object sender, RoutedEventArgs e) {
            this.ClearFocus();

            // Remove the UI button and the model round.
            var index = this.RoundButtonContainer.Children.IndexOf(this.CurrentRoundButton);
            this.InvokeRoundEvent("Remove", index);
            this.RoundButtonContainer.Children.Remove(this.CurrentRoundButton);

            // Make sure there is at least one button and select the last one.
            if (this.RoundButtonContainer.Children.Count == 0) {
                this.AddRoundButton();
            }

            // If there are buttons left, select the last one.
            this.CurrentRoundButton = (Button)this.RoundButtonContainer.Children[^1];

            // Rename buttons
            int i = 1;
            foreach (Button button in this.RoundButtonContainer.Children) {
                button.Content = $"Round {i++}";
            }
        }

        private void HndNewClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();

            if (!this.IsSaved) {
                ConfirmationDialog confDialog = new() {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Text = "League not saved. Do you still want to create a new one?"
                };

                if (confDialog.ShowDialog() == false) return;
            }

            this.Title = "Leagueinator []";
            this.RoundButtonContainer.Children.Clear();
            Button button = this.AddRoundButton();
            this.CurrentRoundButton = button;
            button.Focus();
            this.FileName = "";
            this.IsSaved = true;
        }
        private void HndLoadClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();

            if (!this.IsSaved) {
                ConfirmationDialog confDialog = new() {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Text = "League not saved. Do you still want to load?"
                };

                if (confDialog.ShowDialog() == false) return;
            }

            OpenFileDialog dialog = new OpenFileDialog {
                Filter = "League Files (*.league)|*.league"
            };

            if (dialog.ShowDialog() == true) {
                this.OnFileEvent?.Invoke(this, new FileEventArgs("Load", dialog.FileName));
                this.FileName = dialog.FileName;
                this.IsSaved = true;
            }
        }

        private void HndSaveClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            if (this.FileName == "") this.HndSaveAsClick(null, null);
            this.IsSaved = true;
        }

        private void HndSaveAsClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();

            Microsoft.Win32.SaveFileDialog dialog = new();
            dialog.Filter = "League Files (*.league)|*.league";

            if (dialog.ShowDialog() == true) {
                this.OnFileEvent?.Invoke(this, new FileEventArgs("Save", dialog.FileName));
                this.FileName = dialog.FileName;
                this.IsSaved = true;
            }
        }

        private void HndExitClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.Close();
        }

        private void HndGenEmptyRound(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.AddRoundButton();
            this.InvokeRoundButton();
        }

        private void HndCopyRnd(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.AddRoundButton();
            this.InvokeRoundEvent("Copy");
            this.InvokeRoundButton();
        }
        private void HndShowDataClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeRoundEvent("Show");
        }
        private void HndViewTeamResults(object sender, RoutedEventArgs e) {
            this.ClearFocus();

            PrintWindow pw = new();
            for (int i = 0; i < 10; i++) pw.AddTeam();
            pw.Show();
        }

        private void HndGenNextRound(object sender, RoutedEventArgs args) {
            this.ClearFocus();
        }

        private void HndAssignLanes(object sender, RoutedEventArgs args) {
            this.ClearFocus();
        }

        private void HndMatchAssignments(object sender, RoutedEventArgs e) {
            this.ClearFocus();
        }

        private void HndViewPlayerResults(object sender, RoutedEventArgs e) {
            this.ClearFocus();
        }

        private void HndAssignPlayers(object sender, RoutedEventArgs e) {
            this.ClearFocus();
        }

        private void HndHelpAbout(object sender, RoutedEventArgs e) {
            this.ClearFocus();

            // Get the assembly of the current executing code
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Get the informational version attribute from the assembly
            Version? version = assembly.GetName().Version;

            if (version is not null) {
                DateTime buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
                var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

                string msg = $"{assembly.GetName().Name}\n" +
                             $"Version: {version.Major}.{version.Minor}.{version.Build}\n" +
                             $"Revision: {version.Revision}\n" +
                             $"Build Date: {buildDate}\n";
                MessageBox.Show(msg, "About", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else {
                string msg = "Version Data Not Found";
                MessageBox.Show(msg, "About", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
