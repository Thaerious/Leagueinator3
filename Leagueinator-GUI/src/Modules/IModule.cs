using System.Windows;

namespace Leagueinator.GUI.Modules {
    public interface IModule {
        public void LoadModule(Window targetWindow);

        public void UnloadModule(Window targetWindow);
    }
}
