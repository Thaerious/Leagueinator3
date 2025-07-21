using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Results;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Leagueinator.GUI.Forms.Print {
    /// <summary>
    /// Interaction logic for PrintWindow.xaml
    /// </summary>
    public partial class PrintWindow : Window {
        public PrintWindow(EventData eventData) {
            
            InitializeComponent();

            EventResults eventResults = new EventResults(eventData);
            Debug.WriteLine($"Event Results: {eventResults.ResultsByTeam.Count} teams, {eventResults.ResultsByRound.Count} rounds");

            this.Loaded += (s, e) => {
                foreach (TeamResult teamResult in eventResults.ResultsByTeam) {
                    this.AddTeam(teamResult);
                }
            };
        }

        private void AddTeam(TeamResult teamResult) {
            Section section = new Section();

            // Outer wrapper table for the border
            Table outerTable = new Table {
                Margin = new Thickness(5),
            };

            outerTable.Columns.Add(new TableColumn());

            TableRowGroup outerGroup = new TableRowGroup();
            TableRow outerRow = new TableRow();
            TableCell outerCell = new TableCell {
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Padding = new Thickness(5)
            };

            // Add team header
            string teamHeader = $"[{teamResult.Rank}] {string.Join(", ", teamResult.Players)}";

            outerCell.Blocks.Add(
                new Paragraph(new Run(teamHeader)) {
                    FontWeight = FontWeights.Bold,
                    FontSize = 16,
                    Margin = new Thickness(0)
                }
            );

            // Inner results table
            Table resultsTable = new Table();
            for (int i = 0; i < 6; i++) {
                resultsTable.Columns.Add(
                    new TableColumn { Width = new GridLength(70) }
                );
            }

            TableRowGroup rows = new TableRowGroup();

            // Header row
            rows.Rows.Add(new TableRow {
                Cells = {
                    new TableCell(new Paragraph(new Run("Result"))) { FontWeight = FontWeights.Bold },
                    new TableCell(new Paragraph(new Run("BF")))     { FontWeight = FontWeights.Bold },
                    new TableCell(new Paragraph(new Run("BA")))     { FontWeight = FontWeights.Bold },
                    new TableCell(new Paragraph(new Run("TB")))     { FontWeight = FontWeights.Bold },
                    new TableCell(new Paragraph(new Run("Ends")))   { FontWeight = FontWeights.Bold },
                    new TableCell(new Paragraph(new Run("Lane")))   { FontWeight = FontWeights.Bold },
                    new TableCell(new Paragraph(new Run("VS")))     { FontWeight = FontWeights.Bold },
                }
            });

            // Data rows
            foreach (SingleResult result in teamResult.MatchResults) {
                rows.Rows.Add(this.AddResult(result));
            }

            // Add summary row for the team
            rows.Rows.Add(new TableRow {
                Cells = {
                        new TableCell(new Paragraph(new Run(teamResult.CountWins.ToString()))),
                        new TableCell(new Paragraph(new Run($"{teamResult.BowlsFor}+{teamResult.PlusFor}"))),
                        new TableCell(new Paragraph(new Run($"{teamResult.BowlsAgainst}+{teamResult.PlusAgainst}"))),
                        new TableCell(new Paragraph(new Run(" "))),
                        new TableCell(new Paragraph(new Run($"{teamResult.CountEnds}"))),
                        new TableCell(new Paragraph(new Run($" "))),
                    }
            });

            resultsTable.RowGroups.Add(rows);
            outerCell.Blocks.Add(resultsTable);

            // Wrap up the outer structure
            outerRow.Cells.Add(outerCell);
            outerGroup.Rows.Add(outerRow);
            outerTable.RowGroups.Add(outerGroup);

            section.Blocks.Add(outerTable);
            this.TeamSection.Blocks.Add(section);
        }

        private TableRow AddResult(SingleResult result) {
            List<string> names = [..result.MatchData.GetPlayers()];
            names = [.. names.Except(result.Players)];
            var joinNames = string.Join(", ", names);


            return new TableRow {
                Cells = {
                    new TableCell(new Paragraph(new Run(result.Result.ToString()))),
                    new TableCell(new Paragraph(new Run($"{result.BowlsFor}+{result.PlusFor}"))),
                    new TableCell(new Paragraph(new Run($"{result.BowlsAgainst}+{result.PlusAgainst}"))),
                    new TableCell(new Paragraph(new Run(result.MatchData.TieBreaker == result.TeamIndex ? "✓" : " "))),
                    new TableCell(new Paragraph(new Run($"{result.MatchData.Ends}"))),
                    new TableCell(new Paragraph(new Run($"{result.MatchData.Lane + 1}"))),
                    new TableCell(new Paragraph(new Run($"{joinNames}"))),
                }
            };
        }

        private FlowDocument CloneFlowDocument(FlowDocument original) {
            var range = new TextRange(original.ContentStart, original.ContentEnd);
            using (var ms = new MemoryStream()) {
                range.Save(ms, DataFormats.XamlPackage);
                ms.Position = 0;
                var clone = new FlowDocument();
                var cloneRange = new TextRange(clone.ContentStart, clone.ContentEnd);
                cloneRange.Load(ms, DataFormats.XamlPackage);
                return clone;
            }
        }

        public void Hnd_Print(object sender, RoutedEventArgs e) {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true) {
                try {
                    // CopyRound the FlowDocument to avoid printing the live one
                    FlowDocument clonedDoc = CloneFlowDocument(this.DocViewer);

                    IDocumentPaginatorSource docSource = clonedDoc as IDocumentPaginatorSource;
                    printDialog.PrintDocument(docSource.DocumentPaginator, "Leagueinator Print Job");
                }
                catch (Exception ex) {
                    MessageBox.Show("Print failed: " + ex.Message);
                }
            }
        }

        public void Hnd_Exit(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}

