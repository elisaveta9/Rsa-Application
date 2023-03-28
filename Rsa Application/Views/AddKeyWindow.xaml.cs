using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Rsa_Application
{
    /// <summary>
    /// Логика взаимодействия для AddKeyWindow.xaml
    /// </summary>
    public partial class AddKeyWindow : Window
    {
        public AddKeyWindow()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void MinimazeButton_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;

        private void ExitButton_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
