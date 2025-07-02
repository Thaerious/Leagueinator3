
namespace Leagueinator.GUI.Controllers.Algorithms {
    [Serializable]
    internal class AlgoLogicException : Exception {
        public AlgoLogicException() {
        }

        public AlgoLogicException(string? message) : base(message) {
        }

        public AlgoLogicException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}