using System.Windows.Media;

namespace Leagueinator.GUI.Forms.Main {
    public static class AppColors {
        public static readonly Brush TeamPanelFocused = new SolidColorBrush(Color.FromRgb(0, 200, 0));
        public static readonly Brush TeamPanelDefault = Brushes.Green;
        public static readonly Brush TextBoxAlert = Brushes.LightPink;
        public static readonly Brush LaneAlert = Brushes.Orange;
        public static readonly Brush MatchAlert = Brushes.OrangeRed;
    }
}
