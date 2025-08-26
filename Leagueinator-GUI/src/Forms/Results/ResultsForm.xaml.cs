using Leagueinator.GUI.Controllers.Modules.Motley;
using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Results.BowlsPlus;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Utility;
using Utility.Collections;

namespace Leagueinator.GUI.Forms.Results {
    /// <summary>
    /// Interaction logic for ResultsForm.xaml
    /// </summary>
    public partial class ResultsForm : Window {

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

            this.ShowLeagueResults();
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
            this.ShowLeagueResults();
        }

        private void HndEventToggledOff(object sender, RoutedEventArgs e) {
            if (sender is not CheckBox checkBox) return;
            if (checkBox.Tag is not EventData eventData) return;
            this.WhiteList.Remove(eventData);
            this.ShowLeagueResults();
        }

        private void ShowLeagueResults() {
            this.TeamSection.Blocks.Clear();
            var scores = this.LeagueScores(this.WhiteList);

            int pos = 1;
            foreach ((string Name, List<EventResult> List, EventResult Sum) score in scores) {
                this.AddHeader(
                    [$"#{pos++} {score.Name} ({score.Sum.Score})", "GP", "PTS", "W", "T", "L", "SF", "SA"],
                    [150, 40, 40, 40, 40, 40, 40, 40]
                );

                foreach (EventResult er in score.List) {
                    if (!this.WhiteList.Contains(er.EventData)) continue;

                    var row = this.AddRow(
                        [$"{er.Label}", $"{er.GamesPlayed}", $"{er.Score}", $"{er.Wins}", $"{er.Draws}", $"{er.Losses}", $"{er.ShotsFor}", $"{er.ShotsAgainst}"]
                    );
                }

                this.AddSummaryRow(
                        [$"", $"{score.Sum.GamesPlayed}", $"{score.Sum.Score}", $"{score.Sum.Wins}", $"{score.Sum.Draws}", $"{score.Sum.Losses}", $"{score.Sum.ShotsFor}", $"{score.Sum.ShotsAgainst}"]
                    );

                this.FinishTable(
                    $"diff:{score.Sum.Diff},  pct:{score.Sum.PCT * 100:0.00}"
                );
            }
        }

        private List<(string Name, List<EventResult> List, EventResult Sum)> LeagueScores(IEnumerable<EventData> eventCollection) {
            DefaultDictionary<string, List<EventResult>> dictionary = new((_) => []);

            foreach (EventData eventData in eventCollection) {
                foreach (string name in eventData.AllNames()) {
                    dictionary[name].Add(new(eventData, name));
                }
            }

            return dictionary.Select(kvp => (Name: kvp.Key, List: kvp.Value, Sum: kvp.Value.Sum()))
                             .OrderBy(tuple => tuple.Sum)
                             .ToList();
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
    }
}

