using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Leagueinator.GUI.Utility {
    public static class Logger {
        public static void Log(
            string message,
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0
        ) {
            string filename = System.IO.Path.GetFileName(file);
            Debug.WriteLine($"[{filename}:{line}] {message}");
        }

        public static string GetInvocationLoc(
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0
        ) {
            string filename = System.IO.Path.GetFileName(file);
            return $"[{filename}:{line}]";
        }
    }
}
