
// Thrown when a modules aglorithm fails to produce a valid result.

namespace Algorithms {
    [Serializable]
    public class UnsolvedException : AlgorithmException {
        public UnsolvedException() : base() {
        }

        public UnsolvedException(string? message) : base(message) {
        }

        public UnsolvedException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
