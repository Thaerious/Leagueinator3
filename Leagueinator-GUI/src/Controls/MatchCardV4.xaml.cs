using Leagueinator.GUI.Model;

namespace Leagueinator.GUI.Controls {
    public partial class MatchCardV4 : MatchCard {
        public MatchCardV4() : base() {
            this.InitializeComponent();
        }

        public override MatchFormat MatchFormat { get => MatchFormat.VS4; }
    }
}
