using System.Windows;

namespace Rsa_Application.Infrastructure.Interfaces
{
    internal interface IDialogService
    {
        string FilePath { get; set; }   // путь к выбранному файлу
        bool OpenFileDialog();  // открытие файла
        bool SaveFileDialog();  // сохранение файла
        void ShowMessage(string message);
    }
}
