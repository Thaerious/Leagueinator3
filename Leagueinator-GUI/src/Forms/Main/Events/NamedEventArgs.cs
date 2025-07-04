using Leagueinator.GUI.src.Controllers;

namespace Leagueinator.GUI.Forms.Main {
    public class NamedEventArgs(EventName eventName) : EventArgs {

        public EventName EventName { get; } = eventName;
    }

    public class NamedEventArgs<T>(EventName eventName, T data) : NamedEventArgs(eventName) {
        public T Data { get; } = data;
    }
}
