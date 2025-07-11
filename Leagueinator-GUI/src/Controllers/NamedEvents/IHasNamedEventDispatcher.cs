using Leagueinator.GUI.Controllers.NamedEvents;

namespace Leagueinator.GUI.Controllers.NamedEvents {
    public interface IHasNamedEventDispatcher {
        public NamedEventDispatcher NamedEventDisp { get; set; }
    }
}
