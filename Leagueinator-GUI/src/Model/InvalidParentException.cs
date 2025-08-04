
namespace Leagueinator.GUI.Model {
    [Serializable]
    internal class InvalidParentException : Exception {
        public InvalidParentException() {
        }

        public InvalidParentException(string? message) : base(message) {
        }

        public InvalidParentException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}