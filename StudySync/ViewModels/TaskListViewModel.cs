using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using StudySync.Models;
using StudySync.Services;

namespace StudySync.ViewModels
{
    public class TaskListViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<TaskViewModel> _tasks;
        private ObservableCollection<Subject> _subjects;
        private TaskViewModel _selectedTask;
        private string _selectedSubjectFilter;
        private string _searchText;
        private bool _showCompletedTasks;

        public TaskListViewModel(DatabaseService databaseService)
        {
            if (databaseService == null)
                throw new ArgumentNullException(nameof(databaseService), "Database service cannot be null");

            _databaseService = databaseService;
            System.Diagnostics.Debug.WriteLine("TaskListViewModel constructor: DatabaseService initialized");

            Tasks = new ObservableCollection<TaskViewModel>();
            Subjects = new ObservableCollection<Subject>();
            _selectedSubjectFilter = "All";
            _showCompletedTasks = true;

            // Commands
            AddTaskCommand = new RelayCommand(_ => AddTask());
            EditTaskCommand = new RelayCommand(_ => EditTask(), _ => SelectedTask != null);
            DeleteTaskCommand = new RelayCommand(_ => DeleteTask(), _ => SelectedTask != null);
            CompleteTaskCommand = new RelayCommand(_ => CompleteTask(), _ => SelectedTask != null);

            // Load data after initialization
            try
            {
                RefreshTasks();
                RefreshSubjects();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during TaskListViewModel initialization: {ex.Message}");
            }
        }

        public ObservableCollection<TaskViewModel> Tasks
        {
            get { return _tasks; }
            set { SetProperty(ref _tasks, value); }
        }

        public ObservableCollection<Subject> Subjects
        {
            get { return _subjects; }
            set { SetProperty(ref _subjects, value); }
        }

        public TaskViewModel SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                if (SetProperty(ref _selectedTask, value))
                {
                    ((RelayCommand)EditTaskCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)DeleteTaskCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)CompleteTaskCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string SelectedSubjectFilter
        {
            get { return _selectedSubjectFilter; }
            set
            {
                if (SetProperty(ref _selectedSubjectFilter, value))
                {
                    FilterTasks();
                }
            }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilterTasks();
                }
            }
        }

        public bool ShowCompletedTasks
        {
            get { return _showCompletedTasks; }
            set
            {
                if (SetProperty(ref _showCompletedTasks, value))
                {
                    FilterTasks();
                }
            }
        }

        public ICommand AddTaskCommand { get; }
        public ICommand EditTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand CompleteTaskCommand { get; }

        public void RefreshTasks()
        {
            if (_databaseService == null)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: DatabaseService is null in RefreshTasks");
                return;
            }

            try
            {
                var allTasks = _databaseService.GetAllTasks();
                if (allTasks == null)
                {
                    Tasks.Clear();
                    return;
                }

                var taskViewModels = allTasks.Select(t => new TaskViewModel(t, _databaseService)).ToList();

                int? selectedTaskId = SelectedTask?.Task?.Id;

                Tasks.Clear();
                foreach (var task in taskViewModels)
                {
                    Tasks.Add(task);
                }

                if (selectedTaskId.HasValue)
                {
                    SelectedTask = Tasks.FirstOrDefault(t => t.Task.Id == selectedTaskId.Value);
                }

                FilterTasks();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in RefreshTasks: {ex.Message}");
            }
        }

        public void RefreshSubjects()
        {
            if (_databaseService == null)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: DatabaseService is null in RefreshSubjects");
                return;
            }

            try
            {
                var allSubjects = _databaseService.GetAllSubjects();
                if (allSubjects == null)
                {
                    System.Diagnostics.Debug.WriteLine("WARNING: GetAllSubjects returned null");
                    return;
                }

                Subjects.Clear();
                foreach (var subject in allSubjects)
                {
                    Subjects.Add(subject);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in RefreshSubjects: {ex.Message}");
            }
        }

        private void FilterTasks()
        {
            var filteredTasks = Tasks.ToList();

            if (!ShowCompletedTasks)
            {
                filteredTasks = filteredTasks.Where(t => !t.Task.IsCompleted).ToList();
            }

            if (!string.IsNullOrEmpty(SelectedSubjectFilter) && SelectedSubjectFilter != "All")
            {
                filteredTasks = filteredTasks.Where(t => t.Task.Subject == SelectedSubjectFilter).ToList();
            }

            if (!string.IsNullOrEmpty(SearchText))
            {
                string searchLower = SearchText.ToLower();
                filteredTasks = filteredTasks.Where(t =>
                    t.Task.Title.ToLower().Contains(searchLower) ||
                    (t.Task.Description != null && t.Task.Description.ToLower().Contains(searchLower))).ToList();
            }

            filteredTasks = filteredTasks.OrderBy(t => t.Task.IsCompleted)
                                         .ThenBy(t => t.Task.DueDate)
                                         .ToList();

            Tasks = new ObservableCollection<TaskViewModel>(filteredTasks);
        }

        private void AddTask()
        {
            var newTask = new Models.Task
            {
                Title = "New Task",
                DueDate = DateTime.Now.AddDays(1),
                IsCompleted = false,
                TaskPriority = Priority.Medium
            };

            int id = _databaseService.AddTask(newTask);

            RefreshTasks();

            var newTaskVm = Tasks.FirstOrDefault(t => t.Task.Id == id);
            if (newTaskVm != null)
            {
                SelectedTask = newTaskVm;
                EditTask();
            }
        }

        private void EditTask()
        {
            if (SelectedTask == null)
                return;

            var taskDialog = new TaskEditViewModel(SelectedTask.Task, _databaseService);
            bool? result = ShowTaskEditDialog(taskDialog);

            if (result == true)
            {
                _databaseService.UpdateTask(taskDialog.EditedTask);
                RefreshTasks();
            }
        }

        private void DeleteTask()
        {
            if (SelectedTask == null)
                return;

            bool? result = ShowConfirmDialog("Delete Task", $"Are you sure you want to delete the task '{SelectedTask.Task.Title}'?");

            if (result == true)
            {
                _databaseService.DeleteTask(SelectedTask.Task.Id);
                RefreshTasks();
            }
        }

        private void CompleteTask()
        {
            if (SelectedTask == null)
                return;

            SelectedTask.Task.IsCompleted = !SelectedTask.Task.IsCompleted;
            _databaseService.UpdateTask(SelectedTask.Task);

            RefreshTasks();
        }

        // These methods would connect to UI dialogs - implemented in the View
        protected virtual bool? ShowTaskEditDialog(TaskEditViewModel viewModel)
        {
            return true;
        }

        protected virtual bool? ShowConfirmDialog(string title, string message)
        {
            return true;
        }
    }
}