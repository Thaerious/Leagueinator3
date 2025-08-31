using Leagueinator.GUI.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Utility.Collections;

namespace Leagueinator.GUI.Forms.Results {
    /// <summary>
    /// Interaction logic for ResultsForm.xaml
    /// </summary>
    public partial class ResultsForm : Window {

        public event EventHandler<HashSet<EventData>> CollectionChanged = delegate { };

        private Table? CurrentTable = default;

        private TableRowGroup? RowGroup = default;

        private int[]? Widths = default;

        private LeagueData LeagueData { get; }

        private MultiMap<EventData, TableRow> RowMap = [];

        private HashSet<EventData> WhiteList = [];

        public ResultsForm(string title, LeagueData leagueData) {
            InitializeComponent();
            this.Title = title;
            this.LeagueData = leagueData;
            this.Loaded += this.ResultsFormLoaded;
        }

        private void ResultsFormLoaded(object sender, RoutedEventArgs e) {
            foreach (EventData eventData in this.LeagueData.Events) {
                this.AddEvent(eventData);
            }

            this.CollectionChanged.Invoke(this, this.WhiteList);
        }

        private void AddEvent(EventData eventData) {
            var checkBox = new CheckBox {
                Content = eventData.EventName,
                Margin = new Thickness(5, 2, 5, 2),
                IsChecked = true,
                Tag = eventData
            };

            checkBox.Checked += HndEventToggledOn;
            checkBox.Unchecked += HndEventToggledOff;
            this.EventSelector.Children.Add(checkBox);
            this.WhiteList.Add(eventData);
        }

        private void HndEventToggledOn(object sender, RoutedEventArgs e) {
            if (sender is not CheckBox checkBox) return;
            if (checkBox.Tag is not EventData eventData) return;
            this.WhiteList.Add(eventData);
            this.CollectionChanged.Invoke(this, this.WhiteList);
        }

        private void HndEventToggledOff(object sender, RoutedEventArgs e) {
            if (sender is not CheckBox checkBox) return;
            if (checkBox.Tag is not EventData eventData) return;
            this.WhiteList.Remove(eventData);
            this.CollectionChanged.Invoke(this, this.WhiteList);
        }       

        public void AddHeader(string[] headers, int[] widths) {
            this.Widths = widths;

            this.CurrentTable = new Table() {
                CellSpacing = 0,
                Padding = new Thickness(0),
                Margin = new Thickness(0)
            };

            this.TeamSection.Blocks.Add(this.CurrentTable);
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

        internal TableRow AddRow(string[] cells) {
            if (this.CurrentTable is null) throw new NullReferenceException("Must add header before row.");
            if (this.RowGroup is null) throw new NullReferenceException("Must add header before row.");
            if (cells.Length != this.Widths?.Length) throw new ArgumentOutOfRangeException("Cells length must match widths.");

            var row = new TableRow();
            this.RowGroup.Rows.Add(row);

            for (int i = 0; i < cells.Length; i++) {
                row.Cells.Add(
                    new TableCell(new Paragraph(new Run(cells[i]))) {
                        Padding = new Thickness(0),
                        FontSize = 14,
                        TextAlignment = TextAlignment.Left
                    }
                );
            }

            return row;
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
            this.TeamSection.Blocks.Add(new Paragraph(new Run(text)) {
                FontSize = 12,
                Foreground = Brushes.Gray,
                FontStyle = FontStyles.Italic,
                Margin = new Thickness(0, 0, 0, 0)
            });

            this.TeamSection.Blocks.Add(new Paragraph(new Run("")) {
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

        internal void Clear() {
            this.TeamSection.Blocks.Clear();
        }
    }
}

