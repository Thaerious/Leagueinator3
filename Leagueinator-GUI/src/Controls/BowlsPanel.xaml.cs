using Leagueinator.GUI.Utility.Extensions;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Leagueinator.GUI.Controls.MatchCard;
using static System.Net.Mime.MediaTypeNames;

namespace Leagueinator.GUI.Controls {
    /// <summary>
    /// Interaction logic for BowlsPanel.xaml
    /// </summary>
    public partial class BowlsPanel : UserControl {

        private int _lastCheckedTeamIndex = -1; // Used to track which team is currently checked for tie

        private MatchCard MatchCard {
            get {
                return this.Ancestors<MatchCard>().First() 
                       ?? throw new NullReferenceException("MatchCard not found in BowlsPanel.");
            }
        }

        public BowlsPanel() {
            InitializeComponent();
            this.Loaded += (s, e) => {
                this.CheckTie.Checked += this.MatchCard.HndTieValueChanged;
                this.CheckTie.Unchecked += this.MatchCard.HndTieValueChanged;
            };
        }

        // Force update the binding source when Enter is pressed
        private void TxtEnterPressedHnd(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                if (sender is TextBox textBox) {
                    // Trigger binding update
                    textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();

                    // Move focus To the next control in the tab order
                    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                    textBox.MoveFocus(request);
                }
            }
        }

        private void OnlyNumbers(object sender, TextCompositionEventArgs e) {
            e.Handled = !e.Text.All(char.IsDigit);
        }
    }
}
