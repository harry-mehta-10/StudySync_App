using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StudySync.Models
{
    public class Task : INotifyPropertyChanged
    {
        private int _id;
        private string _title;
        private string _description;
        private DateTime _dueDate;
        private bool _isCompleted;
        private string _subject;
        private DateTime _completedDate;
        private TimeSpan _estimatedTime;
        private Priority _priority;

        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        public DateTime DueDate
        {
            get { return _dueDate; }
            set { SetProperty(ref _dueDate, value); }
        }

        public bool IsCompleted
        {
            get { return _isCompleted; }
            set
            {
                if (SetProperty(ref _isCompleted, value))
                {
                    // If completed, set completed date to now
                    if (value)
                        CompletedDate = DateTime.Now;
                    else
                        CompletedDate = DateTime.MinValue;

                    OnPropertyChanged(nameof(IsOverdue));
                }
            }
        }

        public string Subject
        {
            get { return _subject; }
            set { SetProperty(ref _subject, value); }
        }

        public DateTime CompletedDate
        {
            get { return _completedDate; }
            set { SetProperty(ref _completedDate, value); }
        }

        public TimeSpan EstimatedTime
        {
            get { return _estimatedTime; }
            set { SetProperty(ref _estimatedTime, value); }
        }

        public Priority TaskPriority
        {
            get { return _priority; }
            set { SetProperty(ref _priority, value); }
        }

        public bool IsOverdue => !IsCompleted && DateTime.Now > DueDate;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    public enum Priority
    {
        Low,
        Medium,
        High
    }
}