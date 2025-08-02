using Leagueinator.GUI.Controllers.Modules;
using Leagueinator.GUI.Controllers.NamedEvents;

namespace Leagueinator.GUI.Controllers {
    public partial class MainController {

        private IModule? currentModule = null;

        public void LoadModule(IModule module) {
            if (this.currentModule is not null) {
                currentModule.UnloadModule();
            }

            currentModule = module;
            module.LoadModule(this.Window, this);
            NamedEvent.RegisterHandler(module);
        }
    }
}
