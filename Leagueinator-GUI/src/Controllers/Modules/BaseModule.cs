using Leagueinator.GUI.Forms.Main;
using Leagueinator.GUI.Model;
using System.Windows;

namespace Leagueinator.GUI.Controllers.Modules {
    public abstract class BaseModule : IModule {
        private LeagueData? _leagueData = null;
        public LeagueData LeagueData => _leagueData ?? throw new NullReferenceException("LeagueData not set.");

        private MainWindow? _mainWindow = null;
        public MainWindow MainWindow => _mainWindow ?? throw new NullReferenceException("MainWindow not set.");

        public virtual void LoadModule(Window window, LeagueData leagueData) {
            if (window is not MainWindow mainWindow) {
                throw new ModuleException($"Window is not of type {typeof(MainWindow).Name}");
            }

            this._mainWindow = mainWindow;
            this._leagueData = leagueData;
        }

        public abstract void UnloadModule();
    }
}
