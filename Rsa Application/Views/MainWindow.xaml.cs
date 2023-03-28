using Rsa_Application.Database;
using Rsa_Application.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rsa_Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel mainView;
        public MainWindow()
        {
            InitializeComponent();
            mainView = new MainViewModel();
        }

        private void MinimazeButton_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;

        private void MaximazeButton_Click(object sender, RoutedEventArgs e) =>
            WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;

        private void ExitButton_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
