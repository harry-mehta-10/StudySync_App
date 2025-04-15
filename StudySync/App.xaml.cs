using System;
using System.Windows;
using StudySync.ViewModels;
using StudySync.Services;

namespace StudySync
{
    public partial class App : Application
    {
        public App()
        {
            // Handle unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

#if DEBUG
            // Show debug window
            DebugWindow.Instance.Show();
            System.Diagnostics.Debug.WriteLine("Application initialized in DEBUG mode");
#endif
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            MessageBox.Show($"An unexpected error occurred: {ex?.Message}\n\nPlease restart the application.",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                System.Diagnostics.Debug.WriteLine("Application starting...");

                // Create the database service and ensure it's not null
                var databaseService = new DatabaseService();
                if (databaseService == null)
                {
                    throw new InvalidOperationException("Failed to create DatabaseService");
                }

                System.Diagnostics.Debug.WriteLine("DatabaseService created successfully");

                // Create the main view model with the database service
                var mainViewModel = new MainViewModel(databaseService);
                System.Diagnostics.Debug.WriteLine("MainViewModel created");

                // Create the main window and set its DataContext
                var mainWindow = new MainWindow { DataContext = mainViewModel };
                System.Diagnostics.Debug.WriteLine("MainWindow created");

                // Show the window
                mainWindow.Show();
                System.Diagnostics.Debug.WriteLine("MainWindow shown");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FATAL ERROR during application startup: {ex.Message}");
                MessageBox.Show($"Application startup failed: {ex.Message}\n\n{ex.StackTrace}",
                    "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}