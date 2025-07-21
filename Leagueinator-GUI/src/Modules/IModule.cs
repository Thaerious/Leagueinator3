using Leagueinator.GUI.Model;
using System.Windows;

namespace Leagueinator.GUI.Modules {
    public interface IModule {
        public void LoadModule(Window targetWindow, LeagueData leagueData);

        public void UnloadModule(Window targetWindow);
    }
}
