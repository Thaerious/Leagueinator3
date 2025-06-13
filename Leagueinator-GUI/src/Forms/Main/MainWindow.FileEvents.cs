using System.Windows;

namespace Leagueinator.GUI.Forms.Main {
    public partial class MainWindow : Window {
        /// <summary>
        /// Event args for round data creation.
        /// </summary>
        public class FileEventArgs(string action, string filename) : EventArgs {
            public string Action { get; private set; } = action;
            public string FileName { get; private set; } = filename;
        }

        public event EventHandler<FileEventArgs>? OnFileEvent;
    }
}
