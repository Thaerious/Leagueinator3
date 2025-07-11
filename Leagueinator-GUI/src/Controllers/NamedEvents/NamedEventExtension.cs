using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Utility.Extensions;
using System.Windows;
using System.Xml.Linq;

namespace Leagueinator.GUI.Controllers.NamedEvents {
    public static class NamedEventExtension {
        public static void DispatchNamedEvent(this FrameworkElement element, EventName eventName, ArgTable data) {
            if (element is IHasNamedEventDispatcher window) window.NamedEventDisp.Dispatch(eventName, data);
            element.Ancestors<Window>().FirstOrDefault()?.DispatchNamedEvent(eventName, data);
        }
    }
}
