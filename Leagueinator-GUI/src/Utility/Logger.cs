using System.Diagnostics;

namespace Leagueinator.GUI.Utility {
public static class Logger {
        public static void Log(string message) {
            var stackTrace = new StackTrace(true); // true = capture file info
            var frame = stackTrace.LastLocal();

            var method = frame?.GetMethod();
            var filepath = frame?.GetFileName();
            var line = frame?.GetFileLineNumber();

            if (filepath is not null) {
                filepath = filepath.Split('\\', '/').LastOrDefault(); // Get only the file name, not the full path
            }

            Debug.WriteLine($"[{filepath}:{line}] {message}");
        }

        private static StackFrame? LastLocal(this StackTrace stackTrace) {
            StackFrame? last = null;
            foreach (var frame in stackTrace.GetFrames()) {
                var method = frame.GetMethod();
                var filepath = frame.GetFileName();
                var line = frame.GetFileLineNumber();

                if (filepath is not null) {
                    last = frame;
                }
            }

            return last;
        }

        public static void Trace(string message, bool onlyLocal = true) {
            Debug.WriteLine(message);

            var stackTrace = new StackTrace(true); // true = capture file info
            foreach (var frame in stackTrace.GetFrames()) {
                var method = frame.GetMethod();
                var filepath = frame.GetFileName();
                var line = frame.GetFileLineNumber();

                if (filepath is not null) {
                    filepath = filepath.Split('\\', '/').LastOrDefault(); // Get only the file name, not the full path
                }

                if (!onlyLocal) {
                    Debug.WriteLine($" - [{filepath}:{line}] {method?.DeclaringType?.Name}.{method?.Name}");
                }
                else if(filepath is not null) {
                    Debug.WriteLine($" - [{filepath}:{line}] {method?.DeclaringType?.Name}.{method?.Name}");
                }
            }
        }
    }
}
