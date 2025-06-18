using Leagueinator.GUI.Model;
using System.Windows;

namespace Leagueinator.GUI.Forms.Main {
    public partial class MainWindow : Window {
        public class FileEventArgs(string action) : EventArgs {
            public string Action { get; private set; } = action;
        }

        public event EventHandler<FileEventArgs>? OnFileEvent;

        public void InvokeFileEvent(string action) {
            this.OnFileEvent?.Invoke(this, new FileEventArgs(action));
        }
    }
}
