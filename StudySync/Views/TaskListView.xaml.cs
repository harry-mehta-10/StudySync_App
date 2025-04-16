using System.Windows;
using System.Windows.Controls;
using StudySync.Services;
using StudySync.ViewModels;

namespace StudySync.Views
{
    public partial class TaskListView : UserControl
    {
        public TaskListView()
        {
            InitializeComponent();

            DataContextChanged += (s, e) =>
            {
                if (e.NewValue is TaskListViewModel viewModel)
                {
                    AttachDialogServices(viewModel);
                }
            };
        }

        private void AttachDialogServices(TaskListViewModel viewModel)
        {
            // Ensure that DatabaseService is passed to the ViewModelWithDialogs constructor
            var databaseService = new DatabaseService(); // You might want to retrieve this from DI or another source
            var vmWithDialogs = new TaskListViewModelWithDialogs(viewModel, databaseService);
            DataContext = vmWithDialogs;
        }

        private class TaskListViewModelWithDialogs : TaskListViewModel
        {
            private readonly TaskListViewModel _originalViewModel;

            // Pass the DatabaseService to the base constructor
            public TaskListViewModelWithDialogs(TaskListViewModel viewModel, DatabaseService databaseService)
                : base(databaseService)  // Pass the databaseService to the base constructor
            {
                _originalViewModel = viewModel;

                // Copy properties
                Tasks = _originalViewModel.Tasks;
                Subjects = _originalViewModel.Subjects;
                SelectedTask = _originalViewModel.SelectedTask;
                SelectedSubjectFilter = _originalViewModel.SelectedSubjectFilter;
                SearchText = _originalViewModel.SearchText;
                ShowCompletedTasks = _originalViewModel.ShowCompletedTasks;

                // Forward property changes
                _originalViewModel.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(Tasks))
                        Tasks = _originalViewModel.Tasks;
                    else if (e.PropertyName == nameof(SelectedTask))
                        SelectedTask = _originalViewModel.SelectedTask;
                    else if (e.PropertyName == nameof(Subjects))
                        Subjects = _originalViewModel.Subjects;
                };
            }

            protected override bool? ShowTaskEditDialog(TaskEditViewModel viewModel)
            {
                var dialog = new TaskEditDialog(viewModel);
                return dialog.ShowDialog();
            }

            protected override bool? ShowConfirmDialog(string title, string message)
            {
                return MessageBox.Show(
                    message,
                    title,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes;
            }
        }
    }
}
