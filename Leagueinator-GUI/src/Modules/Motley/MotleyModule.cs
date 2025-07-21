using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Forms.Main;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Utility.Extensions;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Leagueinator.GUI.Modules.Motley {
    public class MotleyModule : IModule {
        private MenuItem ELOMenu = new MenuItem { Header = "ELO" };
        private LeagueData? LeagueData;

        public void LoadModule(Window window, LeagueData leagueData) {
            Debug.WriteLine("Load Motley Module");
            this.LeagueData = leagueData;

            if (window is not MainWindow mainWindow) throw new ModuleException($"Window is not of type {typeof(MainWindow).Name}");
            Hooks.GenerateRound = this.GenerateRound;
            var viewMenu = mainWindow.MainMenu.FindMenuItemByHeader("View") ?? throw new NullReferenceException();
            viewMenu.Items.Add(this.ELOMenu);
            this.ELOMenu.Click += this.EloMenuClick;
        }

        private void EloMenuClick(object sender, RoutedEventArgs e) {
            Dictionary<string, int> elo = ELO.CalculateELO(this.LeagueData);

            string sb = "";
            foreach (var kvp in elo) {
                sb += $"{kvp.Key} : {kvp.Value}\n";
            }

            NamedEvent.Dispatch(EventName.DisplayText, new() {
                ["text"] = sb
            });           
        }

        public void UnloadModule(Window window) {
            Debug.WriteLine("Unload Motley Module");
            if (Hooks.GenerateRound == this.GenerateRound) {
                Hooks.GenerateRound = null;
            }

            if (window is not MainWindow mainWindow) throw new ModuleException($"Window is not of type {typeof(MainWindow).Name}");
            var viewMenu = mainWindow.MainMenu.FindMenuItemByHeader("View") ?? throw new NullReferenceException();
            Debug.WriteLine(viewMenu.Items.Contains(this.ELOMenu));
            viewMenu.Items.Remove(this.ELOMenu);
        }

        public RoundData GenerateRound(EventData eventData) {
            throw new NotImplementedException();
        }
    }
}
