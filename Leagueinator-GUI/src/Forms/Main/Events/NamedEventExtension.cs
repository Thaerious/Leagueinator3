using Leagueinator.GUI.src.Controllers;
using Leagueinator.GUI.Utility.Extensions;
using System.Diagnostics;
using System.Windows;
using System.Xml.Linq;

namespace Leagueinator.GUI.Forms.Main {
    public static class NamedEventExtension {
        public static void InvokeNamedEvent(this FrameworkElement element, EventName eventName, DataTable data) {
            if (element is MainWindow window) window.InvokeNamedEvent(eventName, data);
            element.Ancestors<MainWindow>().FirstOrDefault()?.InvokeNamedEvent(eventName, data);
        }

        public static MainWindow GetMainWindow(this FrameworkElement element) {
            MainWindow? mainWindow = element.Ancestors<MainWindow>().FirstOrDefault();
            if (mainWindow is null) throw new NullReferenceException("Main window not found.");
            return mainWindow;
        }
    }
}
