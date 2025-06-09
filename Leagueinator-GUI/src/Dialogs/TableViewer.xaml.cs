using System.Windows;

namespace Leagueinator.GUI.Dialogs {
    /// <summary>
    /// Interaction logic for TableViewer.xaml
    /// </summary>
    public partial class TableViewer : Window {
        public TableViewer() {
            this.InitializeComponent();
        }

        public void Show(object obj) {
            this.MainTextBlock.Text = obj.ToString();
            this.Show();
        }
    }
}
