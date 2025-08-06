
// Thrown when a modules aglorithm fails to produce a valid result.

namespace Leagueinator.GUI.Controllers {
    [Serializable]
    internal class UnsolvableException : Exception {
        public UnsolvableException() {
        }

        public UnsolvableException(string? message) : base(message) {
        }

        public UnsolvableException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
