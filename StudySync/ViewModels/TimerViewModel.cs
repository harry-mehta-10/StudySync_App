using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace StudySync.ViewModels
{
    public class TimerViewModel : ViewModelBase
    {
        private TimeSpan _timeRemaining;
        private TimeSpan _initialTime;
        private DispatcherTimer _timer;
        private bool _isRunning;
        private bool _isPaused;
        private string _timerName;

        public TimerViewModel()
        {
            _initialTime = TimeSpan.FromMinutes(25); // Default to 25 minutes (pomodoro)
            _timeRemaining = _initialTime;
            _isRunning = false;
            _isPaused = false;
            _timerName = "Study Session";

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;

            // Commands
            StartCommand = new RelayCommand(_ => StartTimer(), _ => !IsRunning || IsPaused);
            PauseCommand = new RelayCommand(_ => PauseTimer(), _ => IsRunning && !IsPaused);
            ResetCommand = new RelayCommand(_ => ResetTimer());
            SetPomodoroCommand = new RelayCommand(_ => SetTime(25));
            SetShortBreakCommand = new RelayCommand(_ => SetTime(5));
            SetLongBreakCommand = new RelayCommand(_ => SetTime(15));
        }

        public TimeSpan TimeRemaining
        {
            get { return _timeRemaining; }
            set { SetProperty(ref _timeRemaining, value); }
        }

        public TimeSpan InitialTime
        {
            get { return _initialTime; }
            set { SetProperty(ref _initialTime, value); }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                if (SetProperty(ref _isRunning, value))
                {
                    ((RelayCommand)StartCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)PauseCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsPaused
        {
            get { return _isPaused; }
            set
            {
                if (SetProperty(ref _isPaused, value))
                {
                    ((RelayCommand)StartCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)PauseCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string TimerName
        {
            get { return _timerName; }
            set { SetProperty(ref _timerName, value); }
        }

        public string TimeDisplay
        {
            get
            {
                return $"{TimeRemaining.Minutes:00}:{TimeRemaining.Seconds:00}";
            }
        }

        public double ProgressPercentage
        {
            get
            {
                if (_initialTime.TotalSeconds <= 0)
                    return 0;

                return (1 - (_timeRemaining.TotalSeconds / _initialTime.TotalSeconds)) * 100;
            }
        }

        public ICommand StartCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand SetPomodoroCommand { get; }
        public ICommand SetShortBreakCommand { get; }
        public ICommand SetLongBreakCommand { get; }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_timeRemaining.TotalSeconds > 0)
            {
                _timeRemaining = _timeRemaining.Subtract(TimeSpan.FromSeconds(1));
                OnPropertyChanged(nameof(TimeRemaining));
                OnPropertyChanged(nameof(TimeDisplay));
                OnPropertyChanged(nameof(ProgressPercentage));
            }
            else
            {
                _timer.Stop();
                IsRunning = false;
                IsPaused = false;

                // Notify user that timer is done
                TimerCompleted?.Invoke(this, EventArgs.Empty);
            }
        }

        private void StartTimer()
        {
            if (!IsRunning || IsPaused)
            {
                _timer.Start();
                IsRunning = true;
                IsPaused = false;
            }
        }

        private void PauseTimer()
        {
            if (IsRunning && !IsPaused)
            {
                _timer.Stop();
                IsPaused = true;
            }
        }

        private void ResetTimer()
        {
            _timer.Stop();
            TimeRemaining = InitialTime;
            IsRunning = false;
            IsPaused = false;
            OnPropertyChanged(nameof(TimeDisplay));
            OnPropertyChanged(nameof(ProgressPercentage));
        }

        private void SetTime(int minutes)
        {
            InitialTime = TimeSpan.FromMinutes(minutes);
            ResetTimer();

            if (minutes == 25)
                TimerName = "Study Session";
            else if (minutes == 5)
                TimerName = "Short Break";
            else if (minutes == 15)
                TimerName = "Long Break";
            else
                TimerName = "Custom Timer";
        }

        // Event for notifying when timer completes
        public event EventHandler TimerCompleted;
    }
}