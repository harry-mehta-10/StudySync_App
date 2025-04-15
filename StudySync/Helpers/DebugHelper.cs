using System;
using System.Diagnostics;
using System.Windows;

namespace StudySync.Helpers
{
    public static class DebugHelper
    {
        public static void Log(string message)
        {
            Debug.WriteLine($"[StudySync] {DateTime.Now:HH:mm:ss.fff}: {message}");
        }

        public static void LogError(Exception ex, string context = "")
        {
            Debug.WriteLine($"[StudySync ERROR] {DateTime.Now:HH:mm:ss.fff}: {context} - {ex.Message}");
            Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

            // Also show a message box in debug mode
#if DEBUG
            MessageBox.Show($"Error in {context}: {ex.Message}\n\n{ex.StackTrace}",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
        }
    }
}