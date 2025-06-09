using Leagueinator.GUI.Controls;
using System.Reflection;
using System.Windows;
using Leagueinator.GUI.Dialogs;

namespace Leagueinator.GUI.Forms.Main {

    [System.Runtime.Versioning.SupportedOSPlatform("windows")] // get rid of CA1416 warning
    public partial class MainWindow : Window {
        public void HndMenuClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
        }

        public void HndSettings(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            SettingsDialog settingsDialog = new (this.EventData);
            settingsDialog.Owner = this;
            settingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (settingsDialog.ShowDialog() == true) {
                this.EventData = settingsDialog.EventData;
            }
        }

        private void HndChangeFormat(object sender, EventArgs e) {
            this.ClearFocus();
            //if (sender is not MenuItem menuItem) return;

            //if (menuItem.Tag is null) return;  // tag is null during initialization
            //if (menuItem.Tag is not string customData) throw new NullReferenceException("Missing tag on context menu item");

            //bool success = Enum.TryParse(customData, out MatchFormat matchFormat);
            //if (!success) throw new ArgumentException("Error on tag on context menu item");

            //foreach (MatchRow matchRow in this.CurrentRoundRow.Matches) {
            //    matchRow.MatchFormat = matchFormat;
            //}
        }

        /// <summary>
        /// Triggered when the delete round button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HndDeleteRnd(object sender, RoutedEventArgs e) {
            this.ClearFocus();

            // Remove the UI button and the model round.
            this.RoundButtonContainer.Children.Remove(this.CurrentRoundButton);

            // Make sure there is at least one button and select the last one.
            if (this.RoundButtonContainer.Children.Count == 0) {
                this.AddRoundButton();
            }

            // If there are buttons left, select the last one.
            this.InvokeRoundButton();

            // Rename buttons
            int i = 1;
            foreach (DataButton<RoundData> button in this.RoundButtonContainer.Children) {
                button.Content = $"Round {i++}";
            }
        }

        private void HndNewClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();

            //if (!this.IsSaved) {
            //    ConfirmationDialog confDialog = new() {
            //        Owner = this,
            //        WindowStartupLocation = WindowStartupLocation.CenterOwner,
            //        Text = "League not saved. Do you still want to create a new one?"
            //    };

            //    if (confDialog.ShowDialog() == false) return;
            //}

            //this.League = NewLeague();
            //this.FileName = "";
            //this.IsSaved = true;
        }
        private void HndLoadClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();

            //if (!this.IsSaved) {
            //    ConfirmationDialog confDialog = new() {
            //        Owner = this,
            //        WindowStartupLocation = WindowStartupLocation.CenterOwner,
            //        Text = "League not saved. Do you still want to load?"
            //    };

            //    if (confDialog.ShowDialog() == false) return;
            //}

            //OpenFileDialog dialog = new OpenFileDialog {
            //    Filter = "League Files (*.league)|*.league"
            //};

            //if (dialog.ShowDialog() == true) {
            //    string text = File.ReadAllText(dialog.FileName);
            //    this.League = new LeagueDecoder(text).Decode();
            //    this.FileName = dialog.FileName;
            //    this.IsSaved = true;
            //}
        }

        private void HndAddMatch(object sender, RoutedEventArgs e) {
            this.ClearFocus();

            //if (this.CurrentRoundRow.Matches.Count == 0) {
            //    this.CurrentRoundRow.Matches.Add(1);
            //}
            //else {
            //    int nextLane = this.CurrentRoundRow.Matches.Select(mr => mr.Lane).Max() + 1;
            //    this.CurrentRoundRow.Matches.Add(nextLane);
            //}
        }

        private void HndSaveClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();

            //if (this.FileName.IsEmpty()) this.HndSaveAsClick(null, null);
            //else File.WriteAllText(this.FileName, this.League.WriteData());
            //this.IsSaved = true;
        }

        private void HndSaveAsClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();

            //Microsoft.Win32.SaveFileDialog dialog = new();
            //dialog.Filter = "League Files (*.league)|*.league";

            //if (dialog.ShowDialog() == true) {
            //    File.WriteAllText(dialog.FileName, this.League.WriteData());
            //    this.FileName = dialog.FileName;
            //    this.IsSaved = true;
            //}
        }

        private void HndExitClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.Close();
        }

        private void HndGenNextRound(object sender, RoutedEventArgs args) {
            this.ClearFocus();

            //RoundRow nextRound = new CopyRound().GenerateRound(this.CurrentRoundRow);
            //new AssignMatchesFactory().Run(nextRound);
            //new LaneAssignerFactory().Run(nextRound);

            //this.AddRoundButton(nextRound);
            //this.InvokeRoundButton();
        }

        private void HndGenEmptyRound(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            this.AddRoundButton();
            this.InvokeRoundButton();
        }

        private void HndCopyRnd(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            RoundData currentData = this.RoundData;
            RoundData newData = this.AddRoundButton(currentData.Copy()).Data!;
            this.InvokeRoundButton();
        }

        private void HndAssignLanes(object sender, RoutedEventArgs args) {
            this.ClearFocus();

            //new LaneAssignerFactory().Run(CurrentRoundRow);
        }

        private void HndMatchAssignments(object sender, RoutedEventArgs e) {
            this.ClearFocus();

            //PrinterForm form = new(new MatchAssignmentsBuilder().BuildElement(this.CurrentRoundRow)) {
            //    Owner = this
            //};

            //form.Show();
        }

        private void HndViewTeamResults(object sender, RoutedEventArgs e) {
            this.ClearFocus();           

            //PlusResults plusResults = new(this.EventRow);
            //TeamXMLBuilder xmlBuilder = new TeamXMLBuilder();
            //Element element = xmlBuilder.BuildElement(plusResults);
            //PrinterForm form = new(element) {
            //    Owner = this
            //};

            //form.Show();
        }

        private void HndViewPlayerResults(object sender, RoutedEventArgs e) {
            this.ClearFocus();

            //PlusResults plusResults = new(this.EventRow);
            //PlayerXMLBuilder xmlBuilder = new PlayerXMLBuilder();
            //Element element = xmlBuilder.BuildElement(plusResults);
            //PrinterForm form = new(element) {
            //    Owner = this
            //};

            //form.Show();
        }

        private void HndEventsClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            //if (this.League is null) return;

            //new TableViewer().Show(this.League.EventTable);
        }

        private void HndShowDataClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            new TableViewer().Show(this.RoundData);
        }

        private void HndMatchesClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            //if (this.League is null) return;

            //new TableViewer().Show(this.League.MatchTable);
        }

        private void HndTeamsClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            //if (this.League is null) return;

            //new TableViewer().Show(this.League.TeamTable);
        }

        private void HndMembersClick(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            //if (this.League is null) return;

            //new TableViewer().Show(this.League.MemberTable);
        }

        private void HndRankedLadder(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            //var factory = new AssignMatchesFactory();
            //factory.Run(this.CurrentRoundRow);
        }

        private void HndRoundRobin(object sender, RoutedEventArgs e) {
            this.ClearFocus();
            //var factory = new RoundRobinFactory();
            //factory.Run(this.CurrentRoundRow);
        }

        private void HndAssignPlayers(object sender, RoutedEventArgs e) {
            this.ClearFocus();


            //foreach (TeamRow teamRow in this.CurrentRoundRow.Teams) {
            //    if (teamRow.Bowls != 0) {
            //        string msg = "Can not assign players in a round that has been played.";
            //        MessageBox.Show(msg, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            //        return;
            //    }
            //}

            //new AssignPlayers().Run(this.EventRow.RoundRows.Last());
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
