using Leagueinator.GUI.Model;
using Utility.Extensions;

namespace Leagueinator.GUI.Controllers.Modules {
    [Serializable]
    internal class UnpairableTeamsException : PreconditionException {
        public List<string> Players { get; private set; } = [];

        public UnpairableTeamsException() {
        }

        public UnpairableTeamsException(IEnumerable<string> team) : base($"Could not pair the team [{team.JoinString()}]") {
            this.Players = [.. team];
        }

        public UnpairableTeamsException(string? message) : base(message) {
        }

        public UnpairableTeamsException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
