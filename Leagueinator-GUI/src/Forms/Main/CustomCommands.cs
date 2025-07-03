using System.Windows.Input;

namespace Leagueinator.GUI.Forms.Main {
    public static class CustomCommands {
        public static readonly RoutedUICommand Swap = new("Swap", "Swap", typeof(CustomCommands));
    }
}
