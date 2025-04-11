using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using StudySync.Services;

namespace StudySync.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private ViewModelBase _currentViewModel;

        public MainViewModel()
        {
            _databaseService = new DatabaseService();

            // Initialize sub view models
            TaskListViewModel = new TaskListViewModel(_databaseService);
            TimerViewModel = new TimerViewModel();
            StatisticsViewModel = new StatisticsViewModel(_databaseService);

            // Default view is task list
            CurrentViewModel = TaskListViewModel;

            // Initialize commands
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