using Leagueinator.GUI.src.Controllers;
using Leagueinator.GUI.Utility.Extensions;
using System.Windows;
using System.Xml.Linq;

namespace Leagueinator.GUI.Forms.Main {
    public static class NamedEventExtension {
        public static void InvokeNamedEvent<T>(this FrameworkElement element, EventName eventName, T data) {
            element.Ancestors<MainWindow>().FirstOrDefault()?.InvokeNamedEvent(eventName, data);
        }
    }
}
