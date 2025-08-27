using System.Reflection;
using Utility;

namespace Leagueinator.GUI.Controllers.NamedEvents {

    record Context(string name);

    public static class NamedEvent {

        private record MethodRecord(object Receiever, MethodInfo Method);

        private static readonly Dictionary<EventName, List<MethodRecord>> Handlers = [];

        private static readonly HashSet<object> Paused = [];

        public static void RegisterHandler(object receiver, bool startPaused = false) {
            Logger.Log($"Register handler: '{receiver.GetType().Name}'");
            if (startPaused) Paused.Add(receiver);

            var methods = receiver.GetType().GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            foreach (MethodInfo method in methods) {
                var attrs = method.GetCustomAttributes<NamedEventHandler>();

                foreach (var attr in attrs) {
                    if (attr is null) continue;
                    if (Handlers.ContainsKey(attr.EventName) == false) {
                        Handlers[attr.EventName] = [];
                    }

                    Handlers[attr.EventName].Add(new(receiver, method));
                    Logger.Log($" - [{attr.EventName}, {method}]'");
                }
            }            
        }

        public static void RemoveHandler(object receiver) {
            Logger.Log($"Remove handler: '{receiver.GetType().Name}'");

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
                    throw new KeyNotFoundException($"Method argument '{p.Name}' for '{receiver.GetType().Name}.{method.Name}' not found for event '{args.EventName}' dispatched by '{args.Source.GetType().Name}'.");
                }
            }

            try {
                Logger.Log($"         '{args.EventName}' from '{args.Source.ContextName()}' handled by '{receiver.GetType().Name}'.");
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
            if (source.IsPaused()) {
                Logger.Log($"         '{eventName}' paused for '{source.ContextName()}'.");
                return;
            }

            Logger.Log($"Event -> '{eventName}' dispatched by '{source.ContextName()}'.");
            data ??= [];

            NamedEventArgs args = new(eventName, source, data);
            InvokeHandlers(args);

            if (args.Handled == false) {
                Logger.Log($"         '{eventName}' dispatched by '{source.ContextName()} not handled.");
            }
        }

        public static bool IsPaused(this object source) {
            var contextAttribute = source.GetType().GetCustomAttribute<NamedEventContext>();

            if (contextAttribute != null) {
                Context context = new(contextAttribute.ContextName);
                if (Paused.Contains(context)) return true;
            }

            if (Paused.Contains(source)) return true;
            return false;
        }

        public static void PauseEvents(this object source) {
            Logger.Log($"Pause handler : {source.ContextName()}");
            var contextAttribute = source.GetType().GetCustomAttribute<NamedEventContext>();
            if (contextAttribute != null) {
                Context context = new(contextAttribute.ContextName);
                Paused.Add(context);
            }
            else {
                Paused.Add(source);
            }
        }

        public static void ResumeEvents(this object source) {
            Logger.Log($"Resume handler : {source.ContextName()}");
            var contextAttribute = source.GetType().GetCustomAttribute<NamedEventContext>();
            if (contextAttribute != null) {
                Context context = new(contextAttribute.ContextName);
                Paused.Remove(context);
            }
            else {
                Paused.Remove(source);
            }
        }

        private static string ContextName(this object source) {
            var contextAttribute = source.GetType().GetCustomAttribute<NamedEventContext>();
            if (contextAttribute != null) {
                return $"{source.GetType().Name}#{contextAttribute.ContextName}";
            }

            return source.GetType().Name;
        }
    }
}
