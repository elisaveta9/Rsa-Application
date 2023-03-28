using Rsa_Application.Infrastructure.Interfaces;
using System.Windows;

namespace Rsa_Application.Common.Services
{
    internal class WindowService : IWindowService
    {
        public void ShowWindow(object window)
        {
            var win = window as Window;
            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ShowDialog();
        }
    }
}
