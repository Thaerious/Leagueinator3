using Leagueinator.GUI.Forms.Print;
using Microsoft.Win32;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Leagueinator.GUI.Forms.Main {

    [System.Runtime.Versioning.SupportedOSPlatform("windows")] // get rid of CA1416 warning
    public partial class MainWindow : Window {

        public void HndMenuClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
        }

        public void HndSettings(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeActionEvent("Settings");
        }

        /// <summary>
        /// Triggered when the delete round button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HndDeleteRnd(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeRoundEvent("Remove");
        }

        private void HndNewClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeFileEvent("New");
        }
        private void HndLoadClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeFileEvent("Load");
        }

        private void HndAssignPlayersRandomly(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeRoundEvent("AssignPlayersRandomly");
        }   

        private void HndSaveClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeFileEvent("Save");
        }

        private void HndSaveAsClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeFileEvent("SaveAs");
        }

        private void HndExitClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.Close();
        }

        private void HndGenEmptyRound(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeRoundEvent(action: "AddRound");
            this.InvokeRoundEvent(action: "Select");
        }

        private void HndCopyRnd(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeRoundEvent("Copy");
        }
        private void HndShowDataClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeRoundEvent("Show");
        }

        private void HndShowRoundResultsClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeRoundEvent("RoundResults");
        }

        private void HndShowEventResultsClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeRoundEvent("EventResults");
        }

        private void HndViewTeamResults(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeActionEvent("PrintTeams");
        }

        private void HndGenNextRound(object sender, RoutedEventArgs args) {
            this.ClearFocus();
            this.InvokeRoundEvent("GenerateRound");
        }

        private void HndAssignLanes(object sender, RoutedEventArgs args) {
            this.ClearFocus();
        }

        private void AssignRankedLadder(object sender, RoutedEventArgs args) {
            this.ClearFocus();
            this.InvokeRoundEvent("AssignRankedLadder");
        }   

        private void AssignRoundRobin(object sender, RoutedEventArgs args) {
            this.ClearFocus();
            this.InvokeRoundEvent("AssignRoundRobin");
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
