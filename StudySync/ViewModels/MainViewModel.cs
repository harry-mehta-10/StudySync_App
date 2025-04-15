using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using StudySync.Services;

namespace StudySync.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        private readonly DatabaseService _databaseService;

        public MainViewModel(DatabaseService databaseService)
        {
            if (databaseService == null)
                throw new ArgumentNullException(nameof(databaseService), "Database service cannot be null");

            _databaseService = databaseService;

            // Create view models with explicit database reference
            TaskListViewModel = new TaskListViewModel(_databaseService);
            TimerViewModel = new TimerViewModel();
            StatisticsViewModel = new StatisticsViewModel(_databaseService);

            // Set default view
            CurrentViewModel = TaskListViewModel;

            // Create commands
            NavigateToTasksCommand = new RelayCommand(_ => NavigateToTasks());
            NavigateToTimerCommand = new RelayCommand(_ => NavigateToTimer());
            NavigateToStatsCommand = new RelayCommand(_ => NavigateToStats());
        }

        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set { SetProperty(ref _currentViewModel, value); }
        }

        public TaskListViewModel TaskListViewModel { get; }
        public TimerViewModel TimerViewModel { get; }
        public StatisticsViewModel StatisticsViewModel { get; }

        public ICommand NavigateToTasksCommand { get; }
        public ICommand NavigateToTimerCommand { get; }
        public ICommand NavigateToStatsCommand { get; }

        private void NavigateToTasks()
        {
            TaskListViewModel.RefreshTasks();
            CurrentViewModel = TaskListViewModel;
        }

        private void NavigateToTimer()
        {
            CurrentViewModel = TimerViewModel;
        }

        private void NavigateToStats()
        {
            StatisticsViewModel.RefreshStats();
            CurrentViewModel = StatisticsViewModel;
        }
    }
}