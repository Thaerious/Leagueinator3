using Leagueinator.GUI.Controllers.Algorithms;
using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Model;
using Leagueinator.Utility.Extensions;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Leagueinator.GUI.Controllers.Modules.Motley {
    public class MotleyModule : BaseModule {
        public override void LoadModule(Window window, MainController mainController) {
            base.LoadModule(window, mainController);
            this.MainWindow.MainMenu.AddMenuItem(["View", "ELO"], this.EloMenuClick);
            this.MainWindow.MainMenu.AddMenuItem(["View", "Export CSV"], this.ExportCSV);
            this.MainWindow.MainMenu.AddMenuItem(["Action", "Generate Next Round"], this.GenerateRound);
        }

        public override void UnloadModule() {
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "ELO"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "Export CSV"]);
            this.MainWindow.MainMenu.RemoveMenuItem(["Action", "Generate Next Round"]);
        }

        private void ExportCSV(object sender, RoutedEventArgs e) {
            SaveFileDialog dialog = new() {
                Filter = "CSV Files (*.csv)|*.csv"
            };

            if (dialog.ShowDialog() == true) {
                var csv = this.ToCSV(this.MainController.LeagueData);
                StreamWriter writer = new(dialog.FileName);
                writer.Write(csv);
                writer.Close();
            }
        }

        private string ToCSV(LeagueData leagueData) {
            string sb = "";

            sb += $"date,name,round,lane,team,score,tb,ends,name\n";

            foreach (EventData eventData in leagueData) {
                foreach (RoundData roundData in eventData) {
                    foreach (MatchData matchData in roundData) {
                        foreach (TeamData teamData in matchData.Teams) {
                            foreach (string name in teamData) {
                                int teamIndex = Array.IndexOf(matchData.Teams, teamData);
                                int[] score = matchData.Score;
                                char[] wlt = LabelWinLossTie(score);
                                sb += $"{eventData.Date},{eventData.EventName},{eventData.IndexOf(roundData)},{matchData.Lane},{teamIndex},{matchData.Score[teamIndex]},{matchData.TieBreaker},{matchData.Ends},{name},{wlt[teamIndex]}\n";
                            }
                        }
                    }
                }
            }

            return sb;
        }

        char[] LabelWinLossTie(int[] values) {
            if (values.Length == 2) {
                if (values[0] == values[1]) return ['T', 'T'];
                if (values[0] > values[1]) return ['W', 'L'];
                return ['L', 'W'];
            }
            else if (values.Length == 4) {
                int[] indices = Enumerable.Range(0, 4)
                    .OrderByDescending(i => values[i])
                    .ToArray();

                char[] result = new char[4];

                result[indices[0]] = 'W';
                result[indices[1]] = 'T';
                result[indices[2]] = 'L';
                result[indices[3]] = 'L';

                return result;
            }
            else throw new Exception($"Length {values.Length} not supported");
        }

        private void GenerateRound(object sender, RoutedEventArgs e) {
            RoundData matchesAssigned = AssignPlayers.Assign(this.MainController.LeagueData, this.MainController.EventData, this.MainController.RoundData);
            matchesAssigned.Fill(this.MainController.EventData);

            try {
                var lanesAssigned = new AssignLanes(this.MainController.EventData, matchesAssigned).Run();
                this.MainController.AddRound(lanesAssigned);
            }
            catch (AlgoLogicException ex) {
                this.DispatchEvent(EventName.Notification, new() {
                    ["message"] = ex.Message
                });
                this.MainController.AddRound(matchesAssigned);
            }
            
        }

        private void EloMenuClick(object sender, RoutedEventArgs e) {
            Dictionary<string, int> elo = ELO.CalculateELO(this.MainController.LeagueData);
            var ordered = elo.OrderBy(kvp => kvp.Value);

            string sb = "";
            foreach (var kvp in ordered) {
                sb += $"{kvp.Key} : {kvp.Value}\n";
            }

            this.DispatchEvent(EventName.DisplayText, new() {
                ["text"] = sb
            });
        }
    }
}
