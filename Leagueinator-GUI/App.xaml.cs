using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Controllers.NamedEvents;
using Leagueinator.GUI.Forms.Main;
using Leagueinator.GUI.src.Controllers;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace Leagueinator.GUI {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            // UI thread exceptions
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            // Non-UI thread exceptions
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            // Setup Event Handlers
            this.Dispatcher.InvokeAsync(() => {
                var mainWindow = (MainWindow)this.MainWindow;

                mainWindow.Loaded += (s, e) => {
                    MainController mainController = new(mainWindow);
                    FocusController focusController = new(mainController);

                    NamedEvent.AddHandler(mainController);
                    NamedEvent.AddHandler(focusController);
                    NamedEvent.AddHandler(new MainWindowReceiver(mainWindow));

                    mainWindow.Ready();
                };
            });
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            var trace = ExceptionPrinter.ToString(e.Exception);
            Debug.WriteLine($"UI Thread Exception:\n");
            Debug.WriteLine(trace);
            MessageBox.Show($"UI Thread Exception:\n\n{trace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            if (e.ExceptionObject is Exception ex) {
                var trace = ExceptionPrinter.ToString(ex);
                Debug.WriteLine($"Background Thread Exception:\n");
                Debug.WriteLine(trace);
                MessageBox.Show($"Background Thread Exception:\n\n{trace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) {
            var trace = ExceptionPrinter.ToString(e.Exception);
            Debug.WriteLine($"Unobserved Task Exception:\n");
            Debug.WriteLine(trace);
            MessageBox.Show($"Unobserved Task Exception:\n\n{trace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.SetObserved();
        }

        protected override void OnExit(ExitEventArgs e) {
            // Cleanup logic
            base.OnExit(e);
            Debug.WriteLine("Application Exit");
        }
    }

    public static class ExceptionPrinter {
        public static string ToString(Exception? ex) {
            string sb = "";
            int depth = 0;

            while (ex != null) {
                sb += $"[{depth}] {ex.GetType().Name}: {ex.Message}\n";

                var trace = new StackTrace(ex, true);
                var frames = trace.GetFrames()?.Where(f => f.GetFileLineNumber() > 0);

                if (frames != null) {
                    foreach (var frame in frames) {
                        string method = frame.GetMethod()?.Name ?? "UnknownMethod";
                        string file = Path.GetFileName(frame.GetFileName()) ?? "UnknownFile";
                        int line = frame.GetFileLineNumber();

                        sb += $" |-- {method}() in {file}:{line}\n";
                    }
                }

                ex = ex.InnerException;
                depth++;
            }
            return sb;
        }

        public static void Print(Exception? ex) {
            int depth = 0;

            while (ex != null) {
                Debug.WriteLine($"[{depth}] {ex.GetType().Name}: {ex.Message}");

                var trace = new StackTrace(ex, true);
                var frames = trace.GetFrames()?.Where(f => f.GetFileLineNumber() > 0);

                if (frames != null) {
                    foreach (var frame in frames) {
                        string method = frame.GetMethod()?.Name ?? "UnknownMethod";
                        string file = Path.GetFileName(frame.GetFileName()) ?? "UnknownFile";
                        int line = frame.GetFileLineNumber();

                        Debug.WriteLine($" |-- {method}() in {file}:{line}");
                    }
                }

                ex = ex.InnerException;
                depth++;
            }
        }
    }
}
