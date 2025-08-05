using Utility.Extensions;
using System.Windows.Controls;
using Leagueinator.Utility.Extensions;

namespace Leagueinator.GUI.Controls.MatchCards {
    public partial class MatchCard4321 : MatchCard {
        public MatchCard4321() : base() {
            this.InitializeComponent();

            this.Loaded += (s, e) => {
                foreach (TextBox textBox in this.IsTagged("Bowls").Cast<TextBox>()) {
                    textBox.PreviewTextInput += InputHandlers.OnlyNumbers;
                }
            };
        }
    }
}
