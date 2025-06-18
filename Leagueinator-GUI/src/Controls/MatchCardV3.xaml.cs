using Leagueinator.GUI.Model;

namespace Leagueinator.GUI.Controls {
    public partial class MatchCardV3 : MatchCard {
        public MatchCardV3() : base() {
            this.InitializeComponent();
        }

        public override MatchFormat MatchFormat { get => MatchFormat.VS3; }
    }
}
