using GemBox.Document;
using Rsa_Application.Common.Services;
using Rsa_Application.ViewModels;
using Rsa_Application.Views;
using System.Windows;

namespace Rsa_Application
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal DisplayRootRegistry displayRootRegistry = new();
        MainViewModel mainWindowViewModel;

        public App()
        {
            displayRootRegistry.RegisterWindowType<MainViewModel, MainWindow>();
            displayRootRegistry.RegisterWindowType<ManagerKeysViewModel, ManagerKeysWindow>();
            displayRootRegistry.RegisterWindowType<AddKeyViewModel, AddKeyWindow>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");

            base.OnStartup(e);

            mainWindowViewModel = new MainViewModel();

            await displayRootRegistry.ShowModalPresentation(mainWindowViewModel);

            Shutdown();
        }
    }
}
