using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Leagueinator.GUI.Forms.Print {
    /// <summary>
    /// Interaction logic for PrintWindow.xaml
    /// </summary>
    public partial class PrintWindow : Window {
        public PrintWindow() {
            InitializeComponent();
        }

        public void AddTeam() {
            var teamBlock = new TeamBlock();
            var container = new BlockUIContainer(teamBlock);
            var section = new Section { BreakPageBefore = false };

            section.Blocks.Add(container);
            this.TeamSection.Blocks.Add(section);
        }

        public void Hnd_Print(object sender, RoutedEventArgs e) {
            var printDialog = new System.Windows.Controls.PrintDialog();
            if (printDialog.ShowDialog() == true) {
                var viewer = this.DocViewer;
                viewer.IsEnabled = false;

                var doc = (IDocumentPaginatorSource)this.DocViewer;
                var paginator = doc.DocumentPaginator;

                printDialog.PrintDocument(paginator, "Leagueinator Print Job");
                this.InvalidateVisual();
                this.UpdateLayout();

                Debug.WriteLine("Print job sent successfully.");
                viewer.IsEnabled = true;
            }
        }

        public void Hnd_Exit(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}

