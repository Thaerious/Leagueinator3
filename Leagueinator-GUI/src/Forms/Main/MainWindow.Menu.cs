using Leagueinator.GUI.Forms.Print;
using Leagueinator.GUI.src.Controllers;
using Microsoft.Win32;
using System.Diagnostics;
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
            this.InvokeNamedEvent(EventName.Settings);
        }

        public void HndSwap(object? sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeNamedEvent(EventName.Swap);
        }

        /// <summary>
        /// Triggered when the delete round button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HndDeleteRnd(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeNamedEvent(EventName.RemoveRound);
        }

        private void HndNewClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeNamedEvent(EventName.New);
        }
        private void HndLoadClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeNamedEvent(EventName.Load);
        }

        private void HndAssignPlayersRandomly(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeNamedEvent(EventName.AssignPlayersRandomly);
        }   

        private void HndSaveClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeNamedEvent(EventName.Save);
        }

        private void HndSaveAsClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeNamedEvent(EventName.SaveAs);
        }

        private void HndExitClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.Close();
        }

        private void HndGenEmptyRound(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeNamedEvent(EventName.AddRound);
            this.InvokeNamedEvent(EventName.SelectRound);
        }

        private void HndCopyRnd(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeNamedEvent(EventName.Copy);
        }
        private void HndShowDataClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeNamedEvent(EventName.Show);
        }

        private void HndShowRoundResultsClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeNamedEvent(EventName.RoundResults);
        }

        private void HndShowEventResultsClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeNamedEvent(EventName.EventResults);
        }

        private void HndViewTeamResults(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.InvokeNamedEvent(EventName.PrintTeams);
        }

        private void HndGenNextRound(object sender, RoutedEventArgs args) {
            this.ClearFocus();
            this.InvokeNamedEvent(EventName.GenerateRound);
        }

        private void HndAssignLanes(object sender, RoutedEventArgs args) {
            this.ClearFocus();
            this.InvokeNamedEvent(EventName.AssignLanes);
        }

        private void HndViewPlayerResults(object sender, RoutedEventArgs e) {
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
