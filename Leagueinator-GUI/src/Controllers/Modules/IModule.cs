using Leagueinator.GUI.Model;
using System.Windows;

namespace Leagueinator.GUI.Controllers.Modules {
    public interface IModule {
        public void LoadModule(Window targetWindow, MainController mainController);

        public void UnloadModule();
    }
}
