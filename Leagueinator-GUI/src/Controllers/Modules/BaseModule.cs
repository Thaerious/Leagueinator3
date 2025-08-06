using Leagueinator.GUI.Forms.Main;
using Leagueinator.GUI.Model;
using System.Windows;

namespace Leagueinator.GUI.Controllers.Modules {
    public abstract class BaseModule : IModule {
        private MainController? _mainController = null;
        public MainController MainController => _mainController ?? throw new NullReferenceException("MainController not set.");

        private MainWindow? _mainWindow = null;
        public MainWindow MainWindow => _mainWindow ?? throw new NullReferenceException("MainWindow not set.");

        public virtual void LoadModule(Window window, MainController mainController) {
            if (window is not MainWindow mainWindow) {
                throw new ModuleException($"Window is not of type {typeof(MainWindow).Name}");
            }

            this._mainWindow = mainWindow;
            this._mainController = mainController;
        }

        public abstract void UnloadModule();
    }
}
