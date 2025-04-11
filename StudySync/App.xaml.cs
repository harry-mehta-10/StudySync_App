using System;
using System.Windows;

namespace StudySync
{
    public partial class App : Application
    {
        public App()
        {
            // Handle any unhandled exceptions to prevent app crashes
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            MessageBox.Show($"An unexpected error occurred: {ex?.Message}\n\nPlease restart the application.",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}