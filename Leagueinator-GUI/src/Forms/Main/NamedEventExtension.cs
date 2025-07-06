using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.src.Controllers;
using Leagueinator.GUI.Utility.Extensions;
using System.Windows;
using System.Xml.Linq;

namespace Leagueinator.GUI.Forms.Main {
    public static class NamedEventExtension {
        public static void DispatchNamedEvent(this FrameworkElement element, EventName eventName, ArgTable data) {
            if (element is MainWindow window) window.NamedEventDisp.Dispatch(eventName, data);
            element.Ancestors<MainWindow>().FirstOrDefault()?.DispatchNamedEvent(eventName, data);
        }

        public static MainWindow GetMainWindow(this FrameworkElement element) {
            MainWindow? mainWindow = element.Ancestors<MainWindow>().FirstOrDefault();
            if (mainWindow is null) throw new NullReferenceException("Main window not found.");
            return mainWindow;
        }
    }
}
