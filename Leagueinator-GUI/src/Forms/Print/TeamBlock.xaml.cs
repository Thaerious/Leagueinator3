using Leagueinator.GUI.Model.Results.BowlsPlus;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Leagueinator.GUI.Forms.Print {
    /// <summary>
    /// Interaction logic for TeamBlock.xaml
    /// </summary>
    public partial class TeamBlock : UserControl {
        public TeamBlock() {
            InitializeComponent();
        }

        public void AddPlayers(string[] players) {
            foreach (string playerName in players) {
                if (string.IsNullOrEmpty(playerName)) {
                    continue; // Skip empty player Players
                }
                var playerTextBlock = new TextBlock {
                    Text = playerName,
                    FontSize = 16,
                    Margin = new Thickness(0, 0, 0, 5) // Add some spacing between Players
                };
                this.PlayerNames.Children.Add(playerTextBlock);
            }
        }

        public void AddResults(List<SingleResult> matchResults) {
            foreach (SingleResult matchResult in matchResults) {
                this.AddResult(matchResult);
            }
        }

        internal void AddResult(SingleResult matchResult) {
            StackPanel stackPanel = new StackPanel {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            string[] values = { 
                matchResult.Result.ToString(), 
                $"{matchResult.BowlsFor}+{matchResult.PlusFor}",
                $"{matchResult.BowlsAgainst}+{matchResult.PlusAgainst}",
                $"{(matchResult.MatchData.TieBreaker == matchResult.TeamIndex ? 'Y' : 'N')}",
                $"{matchResult.MatchData.Ends}",
                $"{matchResult.MatchData.Lane + 1}",
            };

            foreach (string header in values) {
                TextBlock textBlock = new TextBlock {
                    Width = 50,
                    Text = header,
                    TextAlignment = TextAlignment.Center,
                };
                stackPanel.Children.Add(textBlock);
            }

            this.Results.Children.Add(stackPanel);
        }
    }
}
