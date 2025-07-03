using Leagueinator.GUI.Model;

namespace Leagueinator.GUI.Controls {
    public partial class MatchCard4321 : MatchCard {
        public MatchCard4321() : base() {
            this.InitializeComponent();
        }

        public override MatchFormat MatchFormat { get => MatchFormat.A4321; }
    }
}
