using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Rsa_Application.Views
{
    /// <summary>
    /// Логика взаимодействия для ManagerKeysWindow.xaml
    /// </summary>
    public partial class ManagerKeysWindow : Window
    {
        public ManagerKeysWindow()
        {
            InitializeComponent();
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
