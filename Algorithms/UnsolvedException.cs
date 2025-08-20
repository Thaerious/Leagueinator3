
// Thrown when a modules aglorithm fails to produce a valid result.

namespace Algorithms {
    [Serializable]
    internal class UnsolvedException : Exception {
        public UnsolvedException() {
        }

        public UnsolvedException(string? message) : base(message) {
        }

        public UnsolvedException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
