using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Leagueinator.GUI.Forms.Print {
    /// <summary>
    /// Interaction logic for ResultsWindow.xaml
    /// </summary>
    public partial class ResultsWindow : Window {

        Section Section = new Section();

        private Table? CurrentTable = default;

        private TableRowGroup? RowGroup = default;

        private int[]? Widths = default;

        public ResultsWindow(string title) {
            InitializeComponent();
            this.TeamSection.Blocks.Add(Section);
            this.Title = title;
        }

        public void AddHeader(string[] headers, int[] widths) {    
            this.Widths = widths;

            this.CurrentTable = new Table() {
                CellSpacing = 0,           
                Padding = new Thickness(0),
                Margin = new Thickness(0)
            };

            this.Section.Blocks.Add(this.CurrentTable);
            this.RowGroup = new TableRowGroup();
            this.CurrentTable.RowGroups.Add(this.RowGroup);
            var headerRow = new TableRow();
            this.RowGroup.Rows.Add(headerRow);

            for (int i = 0; i < headers.Length; i++) {
                this.CurrentTable.Columns.Add(
                    new TableColumn { Width = new GridLength(this.Widths[i]) }
                );
            }

            for (int i = 0; i < headers.Length; i++) {
                headerRow.Cells.Add(
                    new TableCell(new Paragraph(new Run(headers[i]))) {
                        Padding = new Thickness(0),
                        FontWeight = FontWeights.Bold,
                        FontSize = 12,
                        TextAlignment = TextAlignment.Left
                    }
                );
            }
        }

        internal void AddRow(string[] cells) {
            if (this.CurrentTable is null) throw new NullReferenceException("Must add header before row.");
            if (this.RowGroup is null) throw new NullReferenceException("Must add header before row.");
            if (cells.Length != this.Widths?.Length) throw new ArgumentOutOfRangeException($"Cells length must match widths: {cells.Length} != {this.Widths?.Length}");

            var headerRow = new TableRow();
            this.RowGroup.Rows.Add(headerRow);

            for (int i = 0; i < cells.Length; i++) {
                headerRow.Cells.Add(
                    new TableCell(new Paragraph(new Run(cells[i]))) {
                        Padding = new Thickness(0),
                        FontSize = 14,
                        TextAlignment = TextAlignment.Left
                    }
                );
            }
        }

        internal void AddSummaryRow(string[] cells) {
            if (this.CurrentTable is null) throw new NullReferenceException("Must add header before row.");
            if (this.RowGroup is null) throw new NullReferenceException("Must add header before row.");
            if (cells.Length != this.Widths?.Length) throw new ArgumentOutOfRangeException("Cells length must match widths.");

            var headerRow = new TableRow();
            this.RowGroup.Rows.Add(headerRow);

            for (int i = 0; i < cells.Length; i++) {
                headerRow.Cells.Add(
                    new TableCell(new Paragraph(new Run(cells[i]))) {
                        Padding = new Thickness(0),
                        FontSize = 14,
                        TextAlignment = TextAlignment.Left,
                        FontWeight = FontWeights.Bold,
                    }
                );
            }
        }

        internal void FinishTable(string text) {
            this.Section.Blocks.Add(new Paragraph(new Run(text)) {
                FontSize = 12,
                Foreground = Brushes.Gray,
                FontStyle = FontStyles.Italic,
                Margin = new Thickness(0, 0, 0, 0)
            });

            this.Section.Blocks.Add(new Paragraph(new Run("")) {
                FontSize = 12,
                Foreground = Brushes.Gray,
                FontStyle = FontStyles.Italic,
                Margin = new Thickness(0, 0, 0, 0)
            });

            this.CurrentTable = default;
            this.RowGroup = default;
            this.Widths = default;
        }

        private void Hnd_Print(object sender, RoutedEventArgs e) {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true) {
                // SetPlayer the document to match printer page size
                DocViewer.PageHeight = printDialog.PrintableAreaHeight;
                DocViewer.PageWidth = printDialog.PrintableAreaWidth;
                DocViewer.ColumnWidth = printDialog.PrintableAreaWidth; // prevent column wrapping

                // Print the document
                IDocumentPaginatorSource idpSource = DocViewer;
                printDialog.PrintDocument(idpSource.DocumentPaginator, "Leagueinator Print");
            }
        }

        public void Hnd_Exit(object sender, RoutedEventArgs e) {
            this.Close();
        }

        internal void AddHeader(object value) {
            throw new NotImplementedException();
        }
    }
}

