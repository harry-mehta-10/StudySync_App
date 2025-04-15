using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StudySync.Models;
using StudySync.Services;

namespace StudySync.ViewModels
{
    public class StatisticsViewModel : ViewModelBase
    {
        private readonly DatabaseService _databaseService;
        private Statistics _statistics;
        private ObservableCollection<SubjectStatViewModel> _subjectStats;
        private ObservableCollection<DailyStatViewModel> _dailyStats;
        private ObservableCollection<CalendarDataItem> _calendarData;
        private int _completedTasksToday;
        private int _totalTasksDueToday;

        public StatisticsViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService), "Database service cannot be null");

            _statistics = new Statistics();
            _subjectStats = new ObservableCollection<SubjectStatViewModel>();
            _dailyStats = new ObservableCollection<DailyStatViewModel>();
            _calendarData = new ObservableCollection<CalendarDataItem>();

            RefreshStats();
        }

        public Statistics Statistics
        {
            get { return _statistics; }
            set { SetProperty(ref _statistics, value); }
        }

        public ObservableCollection<SubjectStatViewModel> SubjectStats
        {
            get { return _subjectStats; }
            set { SetProperty(ref _subjectStats, value); }
        }

        public ObservableCollection<DailyStatViewModel> DailyStats
        {
            get { return _dailyStats; }
            set { SetProperty(ref _dailyStats, value); }
        }

        public ObservableCollection<CalendarDataItem> CalendarData
        {
            get { return _calendarData; }
            set { SetProperty(ref _calendarData, value); }
        }

        public int CompletedTasksToday
        {
            get { return _completedTasksToday; }
            set { SetProperty(ref _completedTasksToday, value); }
        }

        public int TotalTasksDueToday
        {
            get { return _totalTasksDueToday; }
            set
            {
                SetProperty(ref _totalTasksDueToday, value);
                OnPropertyChanged(nameof(TodayCompletionPercentage));
            }
        }

        public double TodayCompletionPercentage
        {
            get
            {
                if (TotalTasksDueToday == 0)
                    return 0;
                return (double)CompletedTasksToday / TotalTasksDueToday * 100;
            }
        }

        public void RefreshStats()
        {
            try
            {
                if (_databaseService == null)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: DatabaseService is null in RefreshStats");
                    return;
                }

                // Get statistics from database
                _statistics = _databaseService.GetStatistics();
                OnPropertyChanged(nameof(Statistics));

                // Update calendar data
                UpdateCalendarData();

                // Calculate today's stats
                CalculateTodayStats();

                // Generate subject statistics
                GenerateSubjectStats();

                // Generate daily statistics
                GenerateDailyStats();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in RefreshStats: {ex.Message}");
            }
        }

        private void CalculateTodayStats()
        {
            var allTasks = _databaseService.GetAllTasks();
            DateTime today = DateTime.Today;

            // Tasks due today
            var tasksToday = allTasks.Where(t => t.DueDate.Date == today).ToList();
            TotalTasksDueToday = tasksToday.Count;

            // Tasks completed today
            CompletedTasksToday = tasksToday.Count(t => t.IsCompleted);
        }

        private void GenerateSubjectStats()
        {
            // Get all subjects
            var subjects = _databaseService.GetAllSubjects();

            // Convert to view models
            var subjectStatsVms = subjects.Select(s => new SubjectStatViewModel
            {
                Name = s.Name,
                Color = s.Color,
                CompletedTasks = s.CompletedCount,
                TotalTasks = s.TaskCount
            }).ToList();

            // Update collection
            SubjectStats = new ObservableCollection<SubjectStatViewModel>(subjectStatsVms);
        }

        private void GenerateDailyStats()
        {
            if (_statistics.CompletionsByDate == null || _statistics.CompletionsByDate.Count == 0)
            {
                DailyStats = new ObservableCollection<DailyStatViewModel>();
                return;
            }

            // Get the last 14 days
            var today = DateTime.Today;
            var startDate = today.AddDays(-13);

            var dailyStatsVms = new List<DailyStatViewModel>();

            for (var date = startDate; date <= today; date = date.AddDays(1))
            {
                int completions = 0;
                if (_statistics.CompletionsByDate.TryGetValue(date, out int value))
                {
                    completions = value;
                }

                dailyStatsVms.Add(new DailyStatViewModel
                {
                    Date = date,
                    CompletedTasks = completions
                });
            }

            DailyStats = new ObservableCollection<DailyStatViewModel>(dailyStatsVms);
        }

        private void UpdateCalendarData()
        {
            var items = new List<CalendarDataItem>();

            // Default empty calendar with 30 days
            var today = DateTime.Today;
            for (int i = 0; i < 30; i++)
            {
                var date = today.AddDays(-i);
                int count = 0;

                // Try to get completion count from statistics
                if (_statistics?.CompletionsByDate != null &&
                    _statistics.CompletionsByDate.TryGetValue(date, out int completionCount))
                {
                    count = completionCount;
                }

                // Determine color based on count
                string color = count == 0 ? "#EEEEEE" :
                               count < 3 ? "#C8E6C9" :
                               count < 5 ? "#81C784" :
                               count < 8 ? "#4CAF50" : "#2E7D32";

                items.Add(new CalendarDataItem
                {
                    Date = date,
                    Count = count,
                    Color = color
                });
            }

            CalendarData = new ObservableCollection<CalendarDataItem>(items);
        }
    }

    public class CalendarDataItem
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public string Color { get; set; }
    }

    public class SubjectStatViewModel
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public int CompletedTasks { get; set; }
        public int TotalTasks { get; set; }

        public double CompletionPercentage
        {
            get
            {
                if (TotalTasks == 0)
                    return 0;
                return (double)CompletedTasks / TotalTasks * 100;
            }
        }

        public double CompletionPercent => CompletionPercentage;

        public double ProgressWidth
        {
            get
            {
                // Adjust multiplier as needed for your UI
                return CompletionPercentage * 2;
            }
        }
    }

    public class DailyStatViewModel
    {
        public DateTime Date { get; set; }
        public int CompletedTasks { get; set; }

        public string DateDisplay => Date.ToString("MMM d");

        public double Height
        {
            get
            {
                // Adjust multiplier as needed for your UI
                return CompletedTasks * 15;
            }
        }
    }

    public class CalendarItemViewModel
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public string Color { get; set; }
    }
}