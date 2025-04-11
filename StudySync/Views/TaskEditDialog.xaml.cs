using System.Windows;
using StudySync.ViewModels;

namespace StudySync.Views
{
    public partial class TaskEditDialog : Window
    {
        private readonly TaskEditViewModel _viewModel;

        public TaskEditDialog(TaskEditViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}