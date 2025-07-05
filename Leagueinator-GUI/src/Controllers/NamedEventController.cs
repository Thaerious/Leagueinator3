using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Forms.Main;
using Leagueinator.GUI.Utility;
using System.Diagnostics;
using System.Reflection;

namespace Leagueinator.GUI.Controllers {
    public abstract class NamedEventController {
        public void NamedEventHnd(object? sender, NamedEventArgs args){
            Logger.Log($"{this.GetType().Name}.NamedEventHnd: {args.EventName}");

            Type type = typeof(MainController);
            MethodInfo? method = type.GetMethod($"Do{args.EventName}", BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null) return;

            ParameterInfo[] parameters = method.GetParameters();

            List<object> orderedArgs = [];
            foreach (ParameterInfo p in parameters) {
                if (p.Name is null) continue;

                if (args.Data.ContainsKey(p.Name)) {
                    orderedArgs.Add(args.Data[p.Name]);
                }
                else if (p.HasDefaultValue) {
                    orderedArgs.Add(p.DefaultValue!);
                }
                else {
                    throw new KeyNotFoundException($"Parameter '{p.Name}' for '{this.GetType().Name}.{method.Name}' not found in event data {args.Trace}.");
                }
            }

            method.Invoke(this, [.. orderedArgs]);
            args.Handled = true;
        }
    }
}
