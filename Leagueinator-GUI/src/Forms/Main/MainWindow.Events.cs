using Leagueinator.GUI.src.Controllers;
using System.Windows;

namespace Leagueinator.GUI.Forms.Main {
    public partial class MainWindow : Window {

        public event EventHandler<NamedEventArgs>? OnNamedEvent = delegate { };

        public void InvokeNamedEvent(EventName eventName) {
            NamedEventArgs args = new(eventName);
            this.OnNamedEvent?.Invoke(this, args);
        }

        public void InvokeNamedEvent<T>(EventName eventName, T data) {
            NamedEventArgs<T> args = new(eventName, data);
            this.OnNamedEvent?.Invoke(this, args);
        }
    }
}
