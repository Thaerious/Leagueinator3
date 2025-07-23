using Leagueinator.GUI.Model;

namespace Leagueinator.GUI.Controllers.Modules.Motley {
    internal class MotleyImpl {
        private EventData eventData;

        public MotleyImpl(EventData eventData) {
            this.eventData = eventData;
        }

        internal RoundData GenerateRound() {
            throw new NotImplementedException();
        }
    }
}
