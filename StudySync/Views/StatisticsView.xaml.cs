using System;
using System.Windows.Controls;

namespace StudySync.Views
{
    public partial class StatisticsView : UserControl
    {
        public StatisticsView()
        {
            InitializeComponent();
        }
    }
}
public class CalendarItemViewModel
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
    public string Color { get; set; }
}