
namespace Leagueinator.GUI.Modules {
    [Serializable]
    internal class PreconditionException : Exception {
        public PreconditionException() {
        }

        public PreconditionException(string? message) : base(message) {
        }

        public PreconditionException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
