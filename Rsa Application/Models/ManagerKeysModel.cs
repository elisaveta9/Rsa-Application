using Rsa_Application.Database;
using Rsa_Application.Database.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Rsa_Application.Models
{
    internal class ManagerKeysModel : INotifyPropertyChanged
    {
        private KeysRepository repository;
        private ObservableCollection<Key>? _keys = new();
        private Key sKey;

        public KeysRepository Repository { get => repository; set { repository = value; OnPropertyChanged(); } }
        public ObservableCollection<Key> Keys { get => _keys; set { _keys = value; OnPropertyChanged(); } }
        public Key SelectedKey { get => sKey; set { sKey = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
