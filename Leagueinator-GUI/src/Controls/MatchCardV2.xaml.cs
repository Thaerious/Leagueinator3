
using Leagueinator.GUI.Model;

namespace Leagueinator.GUI.Controls {
    public partial class MatchCardV2 : MatchCard {
        public MatchCardV2() : base() {
            this.InitializeComponent();
        }

        public override MatchFormat MatchFormat { get => MatchFormat.VS2; }
    }
}
