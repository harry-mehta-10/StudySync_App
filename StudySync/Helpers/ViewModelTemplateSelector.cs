using System.Windows;
using System.Windows.Controls;
using StudySync.ViewModels;
using StudySync.Views;

namespace StudySync.Helpers
{
    public class ViewModelTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TaskListTemplate { get; set; }
        public DataTemplate StatisticsTemplate { get; set; }
        public DataTemplate TimerTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is TaskListViewModel)
                return TaskListTemplate;
            if (item is StatisticsViewModel)
                return StatisticsTemplate;
            if (item is TimerViewModel)
                return TimerTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}