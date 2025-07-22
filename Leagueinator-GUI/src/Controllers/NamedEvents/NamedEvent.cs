using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Utility;
using System.Reflection;

namespace Leagueinator.GUI.Controllers.NamedEvents {
    public static class NamedEvent {

        private record MethodRecord(object Receiever, MethodInfo Method);

        private static readonly Dictionary<EventName, List<MethodRecord>> Handlers = [];

        public static void AddHandler(object receiver) {
            var methods = receiver.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            foreach (MethodInfo method in methods) {
                var attr = method.GetCustomAttribute<NamedEventHandler>();
                if (attr is null) continue;
                if (Handlers.ContainsKey(attr.EventName) == false) {
                    Handlers[attr.EventName] = [];
                }

                Handlers[attr.EventName].Add(new(receiver, method));
            }
        }

        public static void RemoveHandler(object receiver) {
            var methods = receiver.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            foreach (MethodInfo method in methods) {
                var attr = method.GetCustomAttribute<NamedEventHandler>();
                if (attr is null) continue;
                if (Handlers.ContainsKey(attr.EventName) == false) {
                    Handlers[attr.EventName] = [];
                }

                Handlers[attr.EventName].Remove(new(receiver, method));
            }
        }

        private static void HandleEvent(object receiver, NamedEventArgs args, MethodInfo method) {
            ParameterInfo[] parameters = method.GetParameters();

            List<object> orderedArgs = [];
            foreach (ParameterInfo p in parameters) {
                if (p.Name is null) continue;

                if (args.Data.TryGetValue(p.Name, out object? value)) {
                    orderedArgs.Add(value);
                }
                else if (p.HasDefaultValue) {
                    orderedArgs.Add(p.DefaultValue!);
                }
                else {
                    throw new KeyNotFoundException($"Parameter '{p.Name}' for '{method.DeclaringType!.GetType().Name}.{method.Name}' not found in event data {args.Trace}.");
                }
            }

            try {
                string argstring = string.Join(", ", args.Data.Select(kv => $"{kv.Key}={kv.Value}"));
                Logger.Log($"Event '{args.EventName}'({argstring}) from '{receiver.GetType().Name}' handled by '{receiver.GetType().Name}'.");
                method.Invoke(receiver, [.. orderedArgs]);
                args.Handled = true;
            }
            catch (Exception ex) {
                string msg = $"Exception while handling named event '{args.EventName}' on '{receiver.GetType().Name}'.";
                throw new Exception(msg, ex);
            }
        }

        public static void InvokeHandlers(NamedEventArgs args) {
            if (Handlers.ContainsKey(args.EventName) == false) return;
            foreach (MethodRecord record in Handlers[args.EventName]) {
                HandleEvent(record.Receiever, args, record.Method);
            }
        }

        /// <summary>
        /// Invokes a named event with no data payload.
        /// Logs a warning if no handler marks it as handled.
        /// </summary>
        public static void Dispatch(this object source, EventName eventName) {
            Dispatch(source, eventName, []);
        }

        /// <summary>
        /// Invokes a named event with a data payload (ArgTable).
        /// Logs a warning if no handler marks it as handled.
        /// </summary>
        public static void Dispatch(this object source, EventName eventName, ArgTable data) {
            Logger.Log($"Event '{eventName}' dispatched by {source.GetType().Name}.");

            NamedEventArgs args = new(eventName, data);
            InvokeHandlers(args);

            if (args.Handled == false) {
                Logger.Log($"Warning: Event not handled '{eventName}'.");
            }
        }
    }
}
