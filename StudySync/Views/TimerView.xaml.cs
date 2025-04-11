using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using StudySync.ViewModels;

namespace StudySync.Views
{
    public partial class TimerView : UserControl
    {
        public TimerView()
        {
            InitializeComponent();

            // Handle timer completion event
            if (DataContext is TimerViewModel vm)
            {
                vm.TimerCompleted += Timer_Completed;
            }
            else
            {
                DataContextChanged += (s, e) =>
                {
                    if (e.OldValue is TimerViewModel oldVm)
                    {
                        oldVm.TimerCompleted -= Timer_Completed;
                    }

                    if (e.NewValue is TimerViewModel newVm)
                    {
                        newVm.TimerCompleted += Timer_Completed;
                    }
                };
            }
        }

        private void Timer_Completed(object sender, System.EventArgs e)
        {
            // Show notification when timer completes
            MessageBox.Show(
                "Your timer has completed!",
                "Time's Up!",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}