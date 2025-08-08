
namespace Leagueinator.GUI.Model.Exceptions {
    [Serializable]
    internal class ModelConstraintException : Exception {
        public ModelConstraintException() {
        }

        public ModelConstraintException(string? message) : base(message) {
        }

        public ModelConstraintException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
