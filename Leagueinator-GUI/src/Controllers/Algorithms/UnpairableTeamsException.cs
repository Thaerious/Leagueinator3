using Leagueinator.GUI.Model;

namespace Leagueinator.GUI.Controllers.Algorithms {
    [Serializable]
    internal class UnpairableTeamsException : PreconditionException {
        public TeamData? Team { get; private set; }

        public UnpairableTeamsException() {
        }

        public UnpairableTeamsException(TeamData team) {
            this.Team = team;
        }

        public UnpairableTeamsException(string? message) : base(message) {
        }

        public UnpairableTeamsException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
