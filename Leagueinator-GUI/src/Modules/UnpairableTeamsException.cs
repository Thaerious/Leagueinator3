using Leagueinator.GUI.Model;

namespace Leagueinator.GUI.Modules {
    [Serializable]
    internal class UnpairableTeamsException : PreconditionException {
        public List<string> Players { get; private set; } = [];

        public UnpairableTeamsException() {
        }

        public UnpairableTeamsException(IEnumerable<string> team) {
            this.Players = [.. team];
        }

        public UnpairableTeamsException(string? message) : base(message) {
        }

        public UnpairableTeamsException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
