using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Leagueinator.GUI.Dialogs {
    /// <summary>
    /// Interaction logic for TextViewer.xaml
    /// </summary>
    public partial class TextViewer : Window {
        public TextViewer() {
            this.InitializeComponent();
        }

        public void Show(object obj) {
            this.MainTextBlock.Text = obj.ToString();
            this.Show();
        }

        public void Append(object obj) {
            this.MainTextBlock.Text += "\n" + obj.ToString();
        }

        public void Hnd_Save(object sender, RoutedEventArgs e) {
            SaveFileDialog dialog = new() {
                Filter = "Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv"
            };

            if (dialog.ShowDialog() == true) {
                StreamWriter writer = new(dialog.FileName);
                writer.Write(this.MainTextBlock.Text);
                writer.Close();
            }
        }

        public void Hnd_Print(object sender, RoutedEventArgs e) {
            //var printDialog = new PrintDialog();
            //if (printDialog.ShowDialog() == true) {
            //    try {
            //        // CopyRound the FlowDocument to avoid printing the live one
            //        FlowDocument clonedDoc = CloneFlowDocument(this.DocViewer);

            //        IDocumentPaginatorSource docSource = clonedDoc as IDocumentPaginatorSource;
            //        printDialog.PrintDocument(docSource.DocumentPaginator, "Leagueinator Print Job");
            //    }
            //    catch (Exception ex) {
            //        MessageBox.Show("Print failed: " + ex.Message);
            //    }
            //}
        }
    }
}
