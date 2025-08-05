using System.Diagnostics;
using System.Windows.Controls;
using Leagueinator.Utility.Extensions;

namespace Leagueinator.GUI.Controls.MatchCards {
    /// <summary>
    /// Interaction logic for BowlsPanel.xaml
    /// </summary>
    public partial class BowlsPanel : UserControl {

        public BowlsPanel() {
            InitializeComponent();
            this.Loaded += (s, e) => {
                MatchCard matchCard = this.Ancestors<MatchCard>().First() ?? throw new NullReferenceException(); ;
                this.CheckTie.Checked += matchCard.HndTieValueChanged;
                this.CheckTie.Unchecked += matchCard.HndTieValueChanged;
                this.Bowls.PreviewTextInput += InputHandlers.OnlyNumbers;
                this.Bowls.KeyDown += InputHandlers.NextControlOnEnter;
            };
        }
    }
}
