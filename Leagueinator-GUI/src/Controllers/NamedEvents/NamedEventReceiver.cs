using Leagueinator.GUI.src.Controllers;
using Leagueinator.GUI.Utility;
using System.Reflection;

namespace Leagueinator.GUI.Controllers.NamedEvents {
    public class NamedEventReceiver(object Target) {

        public void Trigger(object? sender, EventName eventName, ArgTable data) {
            this.NamedEventHnd(sender, new NamedEventArgs(eventName, data));
        }

        public void NamedEventHnd(object? sender, NamedEventArgs args){
            Logger.Log($"{Target.GetType().Name}.NamedEventHnd: {args.EventName}");

            Type type = Target.GetType();
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
                    throw new KeyNotFoundException($"Parameter '{p.Name}' for '{Target.GetType().Name}.{method.Name}' not found in event data {args.Trace}.");
                }
            }

            try {
                method.Invoke(Target, [.. orderedArgs]);
                args.Handled = true;
            }
            catch (Exception ex) {
                string msg = $"Exception while handling named event '{args.EventName}' on '{Target.GetType().Name}'.";
                throw new Exception(msg, ex);
            }
        }
    }
}
