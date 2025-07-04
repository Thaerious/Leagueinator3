using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Forms.Main;
using Leagueinator.GUI.Utility;
using System.Diagnostics;
using System.Reflection;

namespace Leagueinator.GUI.Controllers {
    public abstract class NamedEventController {
        public void NamedEventHnd(object? sender, NamedEventArgs e){
            Logger.Log($"{this.GetType().Name}.NamedEventHnd: {e.EventName}");

            Type type = typeof(MainController);
            MethodInfo? method = type.GetMethod($"Do{e.EventName}", BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null) {
                Debug.WriteLine($"Method 'Do{e.EventName}' not found in {this.GetType().Name}.");
                return;
            }

            method.Invoke(this, [e]);
        }
    }
}
