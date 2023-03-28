using Microsoft.Win32;
using Rsa_Application.Infrastructure.Interfaces;
using System.Windows;

namespace Rsa_Application.Common.Services
{
    internal class DefaultDialogService : IDialogService
    {
        public string FilePath { get; set; }

        public bool OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new();

            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                return true;
            }
            return false;
        }

        public bool SaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "txt files (*.txt)|*.txt|doc file (*.doc)|*.doc|docx file (*.docx)|*.docx|RTF file (*.rtf)|*.rtf|" +
                "xml file (*.xml)|*.xml|HTML file (*.html)|*.html"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                return true;
            }
            return false;
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}
