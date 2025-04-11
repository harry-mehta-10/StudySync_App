using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StudySync.Models
{
    public class Statistics : INotifyPropertyChanged
    {
        private int _totalTasks;
        private int _completedTasks;
        private int _currentStreak;
        private int _longestStreak;
        private Dictionary<string, int> _subjectCompletions;
        private Dictionary<DateTime, int> _completionsByDate;

        public int TotalTasks
        {
            get { return _totalTasks; }
            set
            {
                SetProperty(ref _totalTasks, value);
                OnPropertyChanged(nameof(CompletionPercentage));
            }
        }

        public int CompletedTasks
        {
            get { return _completedTasks; }
            set
            {
                SetProperty(ref _completedTasks, value);
                OnPropertyChanged(nameof(CompletionPercentage));
            }
        }

        public double CompletionPercentage
        {
            get
            {
                if (TotalTasks == 0)
                    return 0;
                return (double)CompletedTasks / TotalTasks * 100;
            }
        }

        public int CurrentStreak
        {
            get { return _currentStreak; }
            set { SetProperty(ref _currentStreak, value); }
        }

        public int LongestStreak
        {
            get { return _longestStreak; }
            set { SetProperty(ref _longestStreak, value); }
        }

        public Dictionary<string, int> SubjectCompletions
        {
            get { return _subjectCompletions; }
            set { SetProperty(ref _subjectCompletions, value); }
        }

        public Dictionary<DateTime, int> CompletionsByDate
        {
            get { return _completionsByDate; }
            set { SetProperty(ref _completionsByDate, value); }
        }

        public Statistics()
        {
            _subjectCompletions = new Dictionary<string, int>();
            _completionsByDate = new Dictionary<DateTime, int>();
        }

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
}