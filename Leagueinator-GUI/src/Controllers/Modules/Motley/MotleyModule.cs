using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Utility.Extensions;
using System.Windows;

namespace Leagueinator.GUI.Controllers.Modules.Motley {
    public class MotleyModule : BaseModule {
        public override void LoadModule(Window window, LeagueData leagueData) {
            base.LoadModule(window, leagueData);
            this.MainWindow.MainMenu.AddMenuItem(["View", "ELO"], this.EloMenuClick);
        }

        public override void UnloadModule() {
            this.MainWindow.MainMenu.RemoveMenuItem(["View", "ELO"]);
        }

        private void EloMenuClick(object sender, RoutedEventArgs e) {
            Dictionary<string, int> elo = ELO.CalculateELO(this.LeagueData);

            string sb = "";
            foreach (var kvp in elo) {
                sb += $"{kvp.Key} : {kvp.Value}\n";
            }

            this.DispatchEvent(EventName.DisplayText, new() {
                ["text"] = sb
            });
        }

        public RoundData GenerateRound(EventData eventData) {
            throw new NotImplementedException();
        }        
    }
}
