using Leagueinator.GUI.Controllers.NamedEvents;
using System.Reflection;
using System.Windows;

namespace Leagueinator.GUI.Forms.Main {

    [System.Runtime.Versioning.SupportedOSPlatform("windows")] // get rid of CA1416 warning
    public partial class MainWindow : Window {

        public void HndSettings(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.DispatchEvent(EventName.Settings);
        }

        public void HndSwap(object? sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.DispatchEvent(EventName.SwapTeams);
        }

        /// <summary>
        /// Triggered when the delete round button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HndDeleteRnd(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.DispatchEvent(EventName.RemoveRound);
        }

        private void HndNewLeagueClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.DispatchEvent(EventName.NewLeague);
        }
        private void HndLoadLeagueClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.DispatchEvent(EventName.LoadLeague);
        }

        private void HndAssignPlayersRandomly(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.DispatchEvent(EventName.AssignPlayersRandomly);
        }   

        private void HndSaveLeagueClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.DispatchEvent(EventName.SaveLeague);
        }

        private void HndSaveAsLeagueClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.DispatchEvent(EventName.SaveLeagueAs);
        }

        private void HndExitClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.Close();
        }

        private void HndGenEmptyRound(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.DispatchEvent(EventName.AddRound);
            this.DispatchEvent(EventName.SelectRound);
        }

        private void HndCopyRnd(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.DispatchEvent(EventName.CopyRound);
        }
        private void HndShowDataClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.DispatchEvent(EventName.ShowData);
        }

        private void HndViewTeamResults(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.DispatchEvent(EventName.PrintTeams);
        }

        private void HndAssignLanes(object sender, RoutedEventArgs args) {
            this.ClearFocus();
            this.DispatchEvent(EventName.AssignLanes);
        }

        private void HndViewPlayerResults(object sender, RoutedEventArgs e) {
            this.ClearFocus();
        }

        private void HndEventManagerClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.DispatchEvent(EventName.ShowEventManager);
        }

        private void HndSelectEventClick(object sender, RoutedEventArgs e) {
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
