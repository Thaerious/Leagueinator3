using Leagueinator.GUI.Utility;
using System.Reflection;

namespace Leagueinator.GUI.Controllers.NamedEvents {
    public class NamedEventReceiver {
        public object MethodSource { get; }

        public NamedEventReceiver() {
            this.MethodSource = this;
        }

        public NamedEventReceiver(object methodSource) {
            this.MethodSource = methodSource;
        }

        public void Trigger(object? sender, EventName eventName, ArgTable data) {
            this.NamedEventHnd(sender, new NamedEventArgs(eventName, data));
        }

        public void NamedEventHnd(object? sender, NamedEventArgs args) {
            Type type = MethodSource.GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            foreach (MethodInfo method in methods) {
                var attr = method.GetCustomAttribute<NamedEventHandler> ();
                if (attr is null) continue;
                if (attr.EventName != args.EventName) continue;
                HandleEvent(sender, args, method);
            }
        }

        private void HandleEvent(object? sender, NamedEventArgs args, MethodInfo method) {
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
                    throw new KeyNotFoundException($"Parameter '{p.Name}' for '{MethodSource.GetType().Name}.{method.Name}' not found in event data {args.Trace}.");
                }
            }

            try {
                string argstring = string.Join(", ", args.Data.Select(kv => $"{kv.Key}={kv.Value}"));
                Logger.Log($"Event '{args.EventName}'({argstring}) from '{sender?.GetType().Name}' handled by '{MethodSource.GetType().Name}'.");
                method.Invoke(MethodSource, [.. orderedArgs]);
                args.Handled = true;
            }
            catch (Exception ex) {
                string msg = $"Exception while handling named event '{args.EventName}' on '{MethodSource.GetType().Name}'.";
                throw new Exception(msg, ex);
            }
        }
    }
}
