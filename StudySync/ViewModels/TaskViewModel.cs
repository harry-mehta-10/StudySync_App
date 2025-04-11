using System;
using System.Windows.Input;
using StudySync.Models;
using StudySync.Services;

namespace StudySync.ViewModels
{
    public class TaskViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private Models.Task _task;

        public TaskViewModel(Models.Task task, DatabaseService databaseService)
        {
            _task = task;
            _databaseService = databaseService;

            ToggleCompletionCommand = new RelayCommand(_ => ToggleCompletion());
        }

        public Models.Task Task
        {
            get { return _task; }
            set { SetProperty(ref _task, value); }
        }

        public string Title
        {
            get { return _task.Title; }
            set
            {
                _task.Title = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return _task.Description; }
            set
            {
                _task.Description = value;
                OnPropertyChanged();
            }
        }

        public DateTime DueDate
        {
            get { return _task.DueDate; }
            set
            {
                _task.DueDate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsOverdue));
                OnPropertyChanged(nameof(DueDateString));
            }
        }

        public string DueDateString
        {
            get
            {
                if (DueDate.Date == DateTime.Today)
                    return "Today";
                if (DueDate.Date == DateTime.Today.AddDays(1))
                    return "Tomorrow";
                if (DueDate.Date == DateTime.Today.AddDays(-1))
                    return "Yesterday";

                return DueDate.ToString("MMM d, yyyy");
            }
        }

        public bool IsCompleted
        {
            get { return _task.IsCompleted; }
            set
            {
                _task.IsCompleted = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsOverdue));
            }
        }

        public bool IsOverdue => _task.IsOverdue;

        public string Subject
        {
            get { return _task.Subject; }
            set
            {
                _task.Subject = value;
                OnPropertyChanged();
            }
        }

        public Priority TaskPriority
        {
            get { return _task.TaskPriority; }
            set
            {
                _task.TaskPriority = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan EstimatedTime
        {
            get { return _task.EstimatedTime; }
            set
            {
                _task.EstimatedTime = value;
                OnPropertyChanged();
            }
        }

        public ICommand ToggleCompletionCommand { get; }

        private void ToggleCompletion()
        {
            IsCompleted = !IsCompleted;
            _databaseService.UpdateTask(_task);
        }

        public void SaveChanges()
        {
            _databaseService.UpdateTask(_task);
        }
    }
}