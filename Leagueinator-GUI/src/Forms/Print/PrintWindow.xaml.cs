using Leagueinator.GUI.Model;
using Leagueinator.GUI.Model.Results;
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
        private RoundDataCollection RoundDataCollection;

        public PrintWindow(RoundDataCollection roundDataCollection) {
            this.RoundDataCollection = roundDataCollection;
            InitializeComponent();

            EventResults eventResults = new EventResults(roundDataCollection);

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
                Margin = new Thickness(5)
            };

            outerTable.Columns.Add(new TableColumn());

            TableRowGroup outerGroup = new TableRowGroup();
            TableRow outerRow = new TableRow();
            TableCell outerCell = new TableCell {
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Padding = new Thickness(10)
            };

            // Add team header
            outerCell.Blocks.Add(new Paragraph(new Run(string.Join(", ", teamResult.Team))) {
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 0)
            });

            // Inner results table
            Table resultsTable = new Table();
            for (int i = 0; i < 6; i++) {
                resultsTable.Columns.Add(new TableColumn { Width = new GridLength(50) });
            }

            TableRowGroup rows = new TableRowGroup();

            // Header row
            rows.Rows.Add(new TableRow {
                Cells = {
                    new TableCell(new Paragraph(new Run("R"))) { FontWeight = FontWeights.Bold },
                    new TableCell(new Paragraph(new Run("F"))) { FontWeight = FontWeights.Bold },
                    new TableCell(new Paragraph(new Run("A"))) { FontWeight = FontWeights.Bold },
                    new TableCell(new Paragraph(new Run("T"))) { FontWeight = FontWeights.Bold },
                    new TableCell(new Paragraph(new Run("E"))) { FontWeight = FontWeights.Bold },
                    new TableCell(new Paragraph(new Run("L"))) { FontWeight = FontWeights.Bold },
                }
            });

            // Data rows
            foreach (var result in teamResult.MatchResults) {
                rows.Rows.Add(new TableRow {
                    Cells = {
                        new TableCell(new Paragraph(new Run(result.Result.ToString()))),
                        new TableCell(new Paragraph(new Run($"{result.BowlsFor}+{result.PlusFor}"))),
                        new TableCell(new Paragraph(new Run($"{result.BowlsAgainst}+{result.PlusAgainst}"))),
                        new TableCell(new Paragraph(new Run(result.MatchData.TieBreaker == result.TeamIndex ? "✓" : " "))),
                        new TableCell(new Paragraph(new Run($"{result.MatchData.Ends}"))),
                        new TableCell(new Paragraph(new Run($"{result.MatchData.Lane + 1}"))),
                    }
                });
            }

            resultsTable.RowGroups.Add(rows);
            outerCell.Blocks.Add(resultsTable);

            // Wrap up the outer structure
            outerRow.Cells.Add(outerCell);
            outerGroup.Rows.Add(outerRow);
            outerTable.RowGroups.Add(outerGroup);

            section.Blocks.Add(outerTable);
            this.TeamSection.Blocks.Add(section);
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
                    // Clone the FlowDocument to avoid printing the live one
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

