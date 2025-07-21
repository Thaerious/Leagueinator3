using Leagueinator.GUI.Modules;

namespace Leagueinator.GUI.Modules.Motley {
    internal class MotleyModule : IModule {
        public void LoadHooks() {
            Hooks.GenerateRound = eventData => {
                MotleyImpl motley = new(eventData);
                return motley.GenerateRound();
            };
        }

        public void LoadMenu() {
        }

        public void UnLoadMenu() {
        }
    }
}
