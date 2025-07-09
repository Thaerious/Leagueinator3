using Leagueinator.GUI.Controllers;
using Leagueinator.GUI.Forms.Main;
using Leagueinator.GUI.src.Controllers;
using System.Diagnostics;
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

                mainWindow.Loaded += (object s, RoutedEventArgs e) => {
                    MainController mainController = new(mainWindow);
                    FocusController focusController = new(mainController);

                    // Initialize Controller listeners for handling UI generated events
                    mainWindow.OnDragEnd += mainController.DragEndHnd;
                    mainWindow.NamedEventDisp += mainController.NamedEventRcv;
                    mainWindow.NamedEventDisp += focusController.NamedEventRcv;

                    // Initialize UI listeners for handling Controller generated events
                    mainController.NamedEventDisp += mainWindow.NamedEventRcv;
                    focusController.NamedEventDisp += mainWindow.NamedEventRcv;

                    mainWindow.Ready();
                };
            });
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            Debug.WriteLine($"UI Thread Exception:\n{e.Exception}");
            MessageBox.Show($"UI Thread Exception:\n\n{e.Exception}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            if (e.ExceptionObject is Exception ex) {
                Debug.WriteLine($"Background Thread Exception:\n{ex}");
                MessageBox.Show($"Background Thread Exception:\n\n{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) {
            Debug.WriteLine($"Unobserved Task Exception:\n{e.Exception}");
            MessageBox.Show($"Unobserved Task Exception:\n\n{e.Exception}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.SetObserved();
        }

        protected override void OnExit(ExitEventArgs e) {
            // Cleanup logic
            base.OnExit(e);
            Debug.WriteLine("Application Exit");
        }
    }

}
