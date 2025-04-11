using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using StudySync.Models;
using StudySync.Services;

namespace StudySync.ViewModels
{
    public class TaskEditViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private Models.Task _editedTask;
        private ObservableCollection<string> _availableSubjects;
        private ObservableCollection<Priority> _availablePriorities;

        public TaskEditViewModel(Models.Task task, DatabaseService databaseService)
        {
            _databaseService = databaseService;

            // Create a copy of the task for editing
            _editedTask = new Models.Task
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                IsCompleted = task.IsCompleted,
                Subject = task.Subject,
                CompletedDate = task.CompletedDate,
                EstimatedTime = task.EstimatedTime,
                TaskPriority = task.TaskPriority
            };

            // Load available subjects
            LoadSubjects();

            // Populate available priorities
            _availablePriorities = new ObservableCollection<Priority>(
                Enum.GetValues(typeof(Priority)).Cast<Priority>());

            // Commands
            SaveCommand = new RelayCommand(_ => SaveTask(), _ => CanSaveTask());
            CancelCommand = new RelayCommand(_ => { /* Handled by dialog result */ });
        }

        public Models.Task EditedTask
        {
            get { return _editedTask; }
        }

        public string Title
        {
            get { return _editedTask.Title; }
            set
            {
                _editedTask.Title = value;
                OnPropertyChanged();
                ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        public string Description
        {
            get { return _editedTask.Description; }
            set
            {
                _editedTask.Description = value;
                OnPropertyChanged();
            }
        }

        public DateTime DueDate
        {
            get { return _editedTask.DueDate; }
            set
            {
                _editedTask.DueDate = value;
                OnPropertyChanged();
            }
        }

        public bool IsCompleted
        {
            get { return _editedTask.IsCompleted; }
            set
            {
                _editedTask.IsCompleted = value;
                OnPropertyChanged();
            }
        }

        public string Subject
        {
            get { return _editedTask.Subject; }
            set
            {
                _editedTask.Subject = value;
                OnPropertyChanged();
            }
        }

        public Priority TaskPriority
        {
            get { return _editedTask.TaskPriority; }
            set
            {
                _editedTask.TaskPriority = value;
                OnPropertyChanged();
            }
        }

        public int EstimatedHours
        {
            get { return (int)_editedTask.EstimatedTime.TotalHours; }
            set
            {
                TimeSpan newTime = new TimeSpan(value, EstimatedMinutes, 0);
                _editedTask.EstimatedTime = newTime;
                OnPropertyChanged();
            }
        }

        public int EstimatedMinutes
        {
            get { return _editedTask.EstimatedTime.Minutes; }
            set
            {
                TimeSpan newTime = new TimeSpan(EstimatedHours, value, 0);
                _editedTask.EstimatedTime = newTime;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> AvailableSubjects
        {
            get { return _availableSubjects; }
            set { SetProperty(ref _availableSubjects, value); }
        }

        public ObservableCollection<Priority> AvailablePriorities
        {
            get { return _availablePriorities; }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private void LoadSubjects()
        {
            var subjects = _databaseService.GetAllSubjects();
            var subjectNames = subjects.Select(s => s.Name).ToList();

            AvailableSubjects = new ObservableCollection<string>(subjectNames);
        }

        private bool CanSaveTask()
        {
            return !string.IsNullOrWhiteSpace(Title);
        }

        private void SaveTask()
        {
            // Actual saving happens in the calling code
        }
    }
}