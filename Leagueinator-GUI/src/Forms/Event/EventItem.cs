using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Controllers.NamedEvents;
using System.ComponentModel;

namespace Leagueinator.GUI.Forms.Event
{
    class EventItem : INotifyPropertyChanged {
        private string _name = "Event";
        private MainController Controller;

        public EventItem(MainController controller) {
            this.Controller = controller;
        }

        public string Name {
            get => _name;
            set {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged(nameof(Name));
                this.Controller.NamedEventRcv.Trigger(this, EventName.RenameEvent, 
                    new(){ 
                        ["name"] = value, 
                        ["uid"] = this.EventUID 
                    }
                );
            }
        }
        required public int EventUID { get; set; }
        required public DateTime Date { get; set; }
        required public int Rounds { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string prop) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
