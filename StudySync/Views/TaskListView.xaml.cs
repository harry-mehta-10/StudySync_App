using System.Windows;
using System.Windows.Controls;
using StudySync.ViewModels;

namespace StudySync.Views
{
    public partial class TaskListView : UserControl
    {
        public TaskListView()
        {
            InitializeComponent();

            // If DataContext is a TaskListViewModel, attach dialog services
            if (DataContext is TaskListViewModel vm)
            {
                AttachDialogServices(vm);
            }
            else
            {
                DataContextChanged += (s, e) =>
                {
                    if (e.NewValue is TaskListViewModel viewModel)
                    {
                        AttachDialogServices(viewModel);
                    }
                };
            }
        }

        private void AttachDialogServices(TaskListViewModel viewModel)
        {
            // Create a derived class that handles dialogs
            var vmWithDialogs = new TaskListViewModelWithDialogs(viewModel);
            DataContext = vmWithDialogs;
        }

        private class TaskListViewModelWithDialogs : TaskListViewModel
        {
            private readonly TaskListViewModel _originalViewModel;

            public TaskListViewModelWithDialogs(TaskListViewModel viewModel)
                : base(null) // Not used, we're delegating
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