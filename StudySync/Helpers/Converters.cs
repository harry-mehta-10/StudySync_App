using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using StudySync.Services;

namespace StudySync.Helpers
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return false;
        }
    }

    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return string.IsNullOrWhiteSpace(stringValue) ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SubjectToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string subjectName && !string.IsNullOrWhiteSpace(subjectName))
            {
                // Use a service to get the color for this subject
                var databaseService = new DatabaseService();
                var subject = databaseService.GetSubjectByName(subjectName);

                if (subject != null && !string.IsNullOrWhiteSpace(subject.Color))
                {
                    try
                    {
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString(subject.Color));
                    }
                    catch { }
                }
            }

            // Default color
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string colorString && !string.IsNullOrWhiteSpace(colorString))
            {
                try
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorString));
                }
                catch { }
            }

            // Default color
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ProgressToPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double progress)
            {
                double angle = progress / 100.0 * 360.0;
                double radians = angle * Math.PI / 180.0;

                double x = 125 + 117 * Math.Sin(radians);
                double y = 125 - 117 * Math.Cos(radians);

                return new Point(x, y);
            }

            return new Point(125, 8);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BarHeightConverter : IMultiValueConverter
    {
        private const double MaxBarHeight = 100.0;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 && values[0] is int completedTasks && values[1] is double containerHeight)
            {
                if (completedTasks <= 0)
                    return 0.0;

                // Scale the bar height based on completed tasks (max height for 5+ tasks)
                double percentage = Math.Min(completedTasks / 5.0, 1.0);
                return percentage * containerHeight * 0.9; // 90% of container height max
            }

            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}