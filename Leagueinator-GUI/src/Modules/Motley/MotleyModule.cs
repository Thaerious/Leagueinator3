using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Forms.Main;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Utility.Extensions;
using System.Diagnostics;
using System.Windows;

namespace Leagueinator.GUI.Modules.Motley {
    public class MotleyModule : IModule {
        private LeagueData? _leagueData = null;
        
        public LeagueData LeagueData {
            get {
                if (_leagueData == null) throw new NullReferenceException("LeagueData not set.");
                return _leagueData;
            }
        }

        public void LoadModule(Window window, LeagueData leagueData) {
            this._leagueData = leagueData;

            if (window is not MainWindow mainWindow) {
                throw new ModuleException($"Window is not of type {typeof(MainWindow).Name}");
            }

            Hooks.GenerateRound = this.GenerateRound;
            mainWindow.MainMenu.AddMenuItem(["View", "ELO"], this.EloMenuClick);
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

        public void UnloadModule(Window window) {
            if (window is not MainWindow mainWindow) {
                throw new ModuleException($"Window is not of type {typeof(MainWindow).Name}");
            }

            if (Hooks.GenerateRound == this.GenerateRound) {
                Hooks.GenerateRound = null;
            }

            bool didRemove = mainWindow.MainMenu.RemoveMenuItem(["View", "ELO"]);            
        }

        public RoundData GenerateRound(EventData eventData) {
            throw new NotImplementedException();
        }        
    }
}
