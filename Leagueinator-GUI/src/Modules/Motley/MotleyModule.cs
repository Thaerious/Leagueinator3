using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Dialogs;
using Leagueinator.GUI.Forms.Main;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Modules.RankedLadder;
using Leagueinator.GUI.Utility.Extensions;
using System.Windows;
using System.Windows.Controls;

namespace Leagueinator.GUI.Modules.Motley {
    public class MotleyModule : IModule {
        private MenuItem ELOMenu = new MenuItem { Header = "ELO" };

        public void LoadModule(Window window) {
            if (window is not MainWindow mainWindow) throw new ModuleException($"Window is not of type {typeof(MainWindow).Name}");
            Hooks.GenerateRound = this.GenerateRound;
            var viewMenu = mainWindow.MainMenu.FindMenuItemByHeader("View") ?? throw new NullReferenceException();
            viewMenu.Items.Add(this.ELOMenu);
            this.ELOMenu.Click += this.EloMenuClick;
        }

        private void EloMenuClick(object sender, RoutedEventArgs e) {
            NamedEvent.Dispatch(EventName.DisplayText, new() {
                ["text"] = "ELO"
            });           
        }

        public void UnloadModule(Window window) {
            if (Hooks.GenerateRound == this.GenerateRound) {
                Hooks.GenerateRound = null;
            }

            if (window is not MainWindow mainWindow) throw new ModuleException($"Window is not of type {typeof(MainWindow).Name}");
            var viewMenu = mainWindow.MainMenu.FindMenuItemByHeader("View") ?? throw new NullReferenceException();
            viewMenu.Items.Remove(this.ELOMenu);

        }

        public RoundData GenerateRound(EventData eventData) {
            RankedLadderImpl rankedLadder = new(eventData);
            return rankedLadder.GenerateRound();
        }
    }
}
