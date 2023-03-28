using Rsa_Application.Database;
using Rsa_Application.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Rsa_Application.Models
{
    enum KLen
    {
        lLen,
        bLen
    }
    internal class AddKeyModel : INotifyPropertyChanged
    {
        private KeysRepository keysRepository;
        private string name;
        private KLen lBut = KLen.lLen;
        private int bits, lbit, count, sBit;
        private static readonly List<int> bBits = new() { 2048, 3072, 4096, 8192 };

        public KeysRepository Repository { get => keysRepository; set { keysRepository = value; OnPropertyChanged(); } }
        public List<int> BBits { get => bBits; }
        public int CountKeys { get => count; set { count = value; OnPropertyChanged(); } }
        public KLen SelectedPathLenght { get => lBut; set { lBut = value; OnPropertyChanged(); } }
        public string Name { get => name; set { name = value; OnPropertyChanged(); } }
        public int Bits { get => bits; set { bits = value; OnPropertyChanged(); } }
        public int LowBits { get => lbit; set { lbit = value; OnPropertyChanged(); } }
        public int BigBits { get => sBit; set { sBit = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
