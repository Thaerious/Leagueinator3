
namespace Leagueinator.GUI.Controllers {
    [Serializable]
    internal class UserNotification : Exception {
        public UserNotification() {
        }

        public UserNotification(string? message) : base(message) {
        }

        public UserNotification(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}