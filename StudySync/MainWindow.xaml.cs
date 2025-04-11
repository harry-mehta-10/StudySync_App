using System.Windows;
using StudySync.ViewModels;

namespace StudySync
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}