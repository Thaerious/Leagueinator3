using System.Diagnostics;
using System.Runtime.CompilerServices;

public static class Tracer {
    public static void Trace([CallerLineNumber] int lineNumber = 0,
                             [CallerMemberName] string memberName = "",
                             [CallerFilePath] string filePath = "") {
        Debug.WriteLine($"Trace: {filePath}:{lineNumber} in {memberName}()");
    }

    public static void TraceFullStack() {
        StackTrace stackTrace = new StackTrace(true); // true = capture file info
        Console.WriteLine("Full Stack Trace:");
        foreach (StackFrame frame in stackTrace.GetFrames()) {
            var method = frame.GetMethod();
            string file = frame.GetFileName() ?? "<unknown file>";
            int line = frame.GetFileLineNumber();
            string methodName = method?.DeclaringType?.FullName + "." + method?.Name;
            Debug.WriteLine($"  at {methodName} in {file}:line {line}");
        }
    }
}
