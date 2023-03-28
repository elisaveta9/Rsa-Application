using Rsa_Application.Database;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Rsa_Application.Models
{
    enum State
    {
        FileText_Source,
        FileText_Crypto
    }
    class MainModel : INotifyPropertyChanged
    {
        private readonly Cryptography.Rsa rsa;
        private Database.Entities.Key? activeKey;
        private KeysRepository repository = new(new Database.Context.KeysDbContext());
        private string fileText, programText, _selectedPath, _stateApp = "[режим приложения: шифрование]";
        private bool canEditFileText, canEditProgText = false, isEnabledRB = true, isEnabledManagerKeysButton = true,
            isEnabledDecryptButton = false, isEnabledEditButton = false, isEnabledEncryptButton = true;
        private State state = State.FileText_Source;

        public Cryptography.Options Options { get => rsa.options; set { rsa.options = value; OnPropertyChanged(); } }
        public Cryptography.Key Key { set { rsa.RsaKey = value; OnPropertyChanged(); } }
        public Database.Entities.Key? ActiveKey { get => activeKey; set { activeKey = value; OnPropertyChanged(); } }
        public KeysRepository Repository { get => repository; set { repository = value; OnPropertyChanged(); } }
        public string FileText { get => fileText; set { fileText = value; OnPropertyChanged(); } }
        public string ProgramText { get => programText; set { programText = value; OnPropertyChanged(); } }
        public string SelectedPath { get => _selectedPath; set { _selectedPath = value; OnPropertyChanged(); } }
        public string StateApp { get => _stateApp; set { _stateApp = value; OnPropertyChanged(); } }
        public bool CanEditFileText { get => canEditFileText; set { canEditFileText = value; OnPropertyChanged(); } }
        public bool CanEditProgText { get => canEditProgText; set { canEditProgText = value; OnPropertyChanged(); } }
        public bool IsEnabledTB { get => isEnabledRB; set { isEnabledRB = value; OnPropertyChanged(); } }
        public bool IsEnabledDecryptButton { get => isEnabledDecryptButton; set { isEnabledDecryptButton = value; OnPropertyChanged(); } }
        public bool IsEnabledManagerKeysButton { get => isEnabledManagerKeysButton; set { isEnabledManagerKeysButton = value; OnPropertyChanged(); } }
        public bool IsEnabledEditButton { get => isEnabledEditButton; set { isEnabledEditButton = value; OnPropertyChanged(); } }
        public bool IsEnabledEncryptButton { get => isEnabledEncryptButton; set { isEnabledEncryptButton = value; OnPropertyChanged(); } }
        public State State { get => state; set { state = value; OnPropertyChanged(); } }

        public string Encrypt(string plaintext)
        {
            return rsa.Encrypt(plaintext);
        }
        public string Decrypt(string ciphertext) => rsa.Decrypt(ciphertext);

        public MainModel() =>
            rsa = new Cryptography.Rsa();        

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
