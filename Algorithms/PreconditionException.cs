
// Thrown when a modules aglorithm fails to produce a valid result.

namespace Algorithms {
    [Serializable]
    public class PreconditionException : AlgorithmException {
        public PreconditionException() {
        }

        public PreconditionException(string? message) : base(message) {
        }

        public PreconditionException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
