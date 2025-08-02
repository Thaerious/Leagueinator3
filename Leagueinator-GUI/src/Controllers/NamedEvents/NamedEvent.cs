using Utility;
using System.Reflection;
using System.Diagnostics;

namespace Leagueinator.GUI.Controllers.NamedEvents {
    public static class NamedEvent {

        private record MethodRecord(object Receiever, MethodInfo Method);

        private static readonly Dictionary<EventName, List<MethodRecord>> Handlers = [];

        private static readonly HashSet<object> Paused = [];

        public static void RegisterHandler(object receiver, bool startPaused = false) {
            Logger.Log($"Register handler: '{receiver.GetType().Name}'");
            if (startPaused) Paused.Add(receiver);

            var methods = receiver.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            foreach (MethodInfo method in methods) {
                var attr = method.GetCustomAttribute<NamedEventHandler>();
                if (attr is null) continue;
                if (Handlers.ContainsKey(attr.EventName) == false) {
                    Handlers[attr.EventName] = [];
                }

                Handlers[attr.EventName].Add(new(receiver, method));
                Logger.Log($" - [{attr.EventName}, {method}]'");
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
                Logger.Log($"Event <- '{args.EventName}'({argstring}) from '{args.Source.GetType().Name}' handled by '{receiver.GetType().Name}'.");
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
        /// Invokes a named event with a data payload (ArgTable).
        /// Logs a warning if no handler marks it as handled.
        /// </summary>
        public static void DispatchEvent(this object source, EventName eventName, ArgTable? data = null) {
            if (Paused.Contains(source)) return;

            Logger.Log($"Event -> '{eventName}' dispatched by '{source.GetType().Name}'.");
            data ??= [];

            NamedEventArgs args = new(eventName, source, data);
            InvokeHandlers(args);

            if (args.Handled == false) {
                Logger.Log($"Event <> '{eventName}' dispatched by '{source.GetType().Name} not handled.");
            }
        }

        public static void PauseEvents(this object source) {
            Paused.Add(source); 
        }

        public static void ResumeEvents(this object source) {
            Paused.Remove(source);
        }
    }
}
