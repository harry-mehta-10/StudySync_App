using System;
using System.Windows;
using System.Windows.Controls;

namespace StudySync
{
    public class DebugWindow : Window
    {
        private readonly TextBox _logTextBox;
        private static DebugWindow _instance;

        public static DebugWindow Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DebugWindow();
                return _instance;
            }
        }

        public DebugWindow()
        {
            Title = "Debug Log";
            Width = 800;
            Height = 400;

            _logTextBox = new TextBox
            {
                IsReadOnly = true,
                AcceptsReturn = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Margin = new Thickness(10)
            };

            Content = _logTextBox;
        }

        public void WriteLine(string message)
        {
            Dispatcher.Invoke(() =>
            {
                _logTextBox.Text += $"[{DateTime.Now:HH:mm:ss}] {message}\n";
                _logTextBox.ScrollToEnd();
            });
        }
    }
}