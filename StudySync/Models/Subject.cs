using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace StudySync.Models
{
    public class Subject : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private string _color;
        private int _taskCount;
        private int _completedCount;

        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string Color
        {
            get { return _color; }
            set { SetProperty(ref _color, value); }
        }

        public int TaskCount
        {
            get { return _taskCount; }
            set
            {
                SetProperty(ref _taskCount, value);
                OnPropertyChanged(nameof(CompletionPercentage));
            }
        }

        public int CompletedCount
        {
            get { return _completedCount; }
            set
            {
                SetProperty(ref _completedCount, value);
                OnPropertyChanged(nameof(CompletionPercentage));
            }
        }

        public double CompletionPercentage
        {
            get
            {
                if (TaskCount == 0)
                    return 0;
                return (double)CompletedCount / TaskCount * 100;
            }
        }

        public Brush ColorBrush
        {
            get
            {
                try
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString(Color));
                }
                catch
                {
                    return Brushes.Gray;
                }
            }
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