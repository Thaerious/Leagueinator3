using Leagueinator.GUI.Utility.Extensions;
using System.Windows.Controls;

namespace Leagueinator.GUI.Controls {
    /// <summary>
    /// Interaction logic for BowlsPanel.xaml
    /// </summary>
    public partial class BowlsPanel : UserControl {

        public BowlsPanel() {
            InitializeComponent();
            this.Loaded += (s, e) => {
                this.CheckTie.Checked += this.MatchCard.HndTieValueChanged;
                this.CheckTie.Unchecked += this.MatchCard.HndTieValueChanged;
                this.Bowls.PreviewTextInput += InputHandlers.OnlyNumbers;
                this.Bowls.KeyDown += InputHandlers.TxtNextOnEnter;
            };
        }

        public void SetBowls(int value) {
            this.Bowls.Text = value.ToString();
        }

        public MatchCard MatchCard => this.Ancestors<MatchCard>().First();
    }
}
