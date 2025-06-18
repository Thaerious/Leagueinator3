using Leagueinator.GUI.Model;

namespace Leagueinator.GUI.Controls {
    public partial class MatchCardV1 : MatchCard {
        public MatchCardV1() : base() {
            this.InitializeComponent();
        }

        public override MatchFormat MatchFormat { get => MatchFormat.VS1; }
    }
}
