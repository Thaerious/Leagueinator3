using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Model;
using Leagueinator.Utility.Extensions;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace Leagueinator.GUI.Controllers.Modules.AssignLanes {
    public class CSVModule: BaseModule {
        public override void LoadModule(Window window, MainController mainController) {
            base.LoadModule(window, mainController);
            this.MainWindow.MainMenu.AddMenuItem(["Action", "Load CSV"], this.LoadCSV);
            NamedEvent.RegisterHandler(this);
        }

        public override void UnloadModule() {
            this.MainWindow.MainMenu.RemoveMenuItem(["Action", "Load CSV"]);
            NamedEvent.RemoveHandler(this);
        }

        private void LoadCSV(object sender, RoutedEventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog {
                Filter = "CSV (*.csv)|*.csv"
            };

            if (dialog.ShowDialog() == true) {
                int lineno = 0;
                foreach (var line in File.ReadLines(dialog.FileName)) {
                    if (++lineno == 1) continue;
                    var fields = line.Split(',');
                    int eventIndex = int.Parse(fields[1]);
                    string name = fields[2];                    
                    int round = int.Parse(fields[3]) - 1;
                    MatchFormat format = Enum.Parse<MatchFormat>(fields[4]);
                    int lane = int.Parse(fields[5]) - 1;
                    int team = int.Parse(fields[6]) - 1;
                    int bowls = int.Parse(fields[7]);
                    int ends = int.Parse(fields[8]);

                    if (this.MainController.LeagueData.Events.Count <= eventIndex) {
                        EventData newEvent = this.MainController.LeagueData.AddEvent();
                        newEvent.EventName = fields[0];
                    }

                    EventData eventData = this.MainController.LeagueData.Events[eventIndex];
                    if (eventData.Rounds.Count <= round) eventData.AddRound(false);
                    RoundData roundData = eventData.Rounds[round];

                    if (roundData.Matches.Count <= lane) {
                        roundData.Fill(lane + 1);
                    }

                    MatchData matchData = roundData.Matches[lane];

                    if (matchData.MatchFormat != format) {
                        roundData.InsertMatch(lane, new(roundData) {
                            MatchFormat = format,
                            Ends = ends
                        });
                        matchData = roundData.Matches[lane];
                    }

                    matchData.Teams[team].AddPlayer(name);
                    matchData.Teams[team].Bowls = bowls;                    
                }
                this.MainController.DispatchModel();
            }
        }
    }
}
