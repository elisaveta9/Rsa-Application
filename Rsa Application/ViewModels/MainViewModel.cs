using Rsa_Application.Common.Abstractions;
using Rsa_Application.Common.Services;
using Rsa_Application.Database;
using Rsa_Application.Infrastructure.Interfaces;
using Rsa_Application.Models;
using System;
using System.Windows;

namespace Rsa_Application.ViewModels
{
    class MainViewModel : ViewModel
    {
        private readonly MainModel activeRsaModel = new();

        readonly IFileService fileService;
        readonly IDialogService dialogService;
        readonly IWindowService childWindow;

        public MainViewModel()
        {
            dialogService = new DefaultDialogService();
            fileService = new FileServis();
            childWindow = new WindowService();
            activeRsaModel = new MainModel();
            FileText = ""; ProgramText = ""; SelectedPath = "";
            ActiveKey = Repository.GetActiveKey();
            activeRsaModel.Options = Cryptography.Options.RSAES_OAEP;
            CanEditFileText = true;
        }

        #region Свойства
        public bool CanEditFileText { get => !activeRsaModel.CanEditFileText; set { activeRsaModel.CanEditFileText = value; OnPropertyChanged(); } }
        public bool CanEditProgText { get => !activeRsaModel.CanEditProgText; set { activeRsaModel.CanEditProgText = value; OnPropertyChanged(); }  }
        public bool IsSimpleRSA { get { return Options == Cryptography.Options.SimpleRSA; }
            set { Options = value ? Cryptography.Options.SimpleRSA : Options; OnPropertyChanged(); } }
        public bool IsRSAAdd
        { get { return Options == Cryptography.Options.RSA_withAddBytes; }
            set { Options = value ? Cryptography.Options.RSA_withAddBytes : Options; OnPropertyChanged(); } }
        public bool IsRSAComp
        { get { return Options == Cryptography.Options.RSA_withCompBytes; }
            set { Options = value ? Cryptography.Options.RSA_withCompBytes : Options; OnPropertyChanged(); } }
        public bool IsRSAEP_OAEP
        { get { return Options == Cryptography.Options.RSAES_OAEP; }
            set { Options = value ? Cryptography.Options.RSAES_OAEP : Options; OnPropertyChanged(); } }
        public bool IsEnabledManagerKeysButton { get => activeRsaModel.IsEnabledManagerKeysButton; set { activeRsaModel.IsEnabledManagerKeysButton = value; OnPropertyChanged(); } }
        public bool IsEnabledDecryptButton { get => activeRsaModel.IsEnabledDecryptButton; set { activeRsaModel.IsEnabledDecryptButton = value; OnPropertyChanged(); } }
        public bool IsEnabledEditButton { get => activeRsaModel.IsEnabledEditButton; set { activeRsaModel.IsEnabledEditButton = value; OnPropertyChanged(); } }
        public bool IsEnabledEncryptButton { get => activeRsaModel.IsEnabledEncryptButton; set { activeRsaModel.IsEnabledEncryptButton = value; OnPropertyChanged(); } }
        public bool IsEnabledRadioButton { get => activeRsaModel.IsEnabledTB; set { activeRsaModel.IsEnabledTB = value; OnPropertyChanged(); } }
        public string SelectedPath { get => activeRsaModel.SelectedPath; set { activeRsaModel.SelectedPath = value; OnPropertyChanged(); } }
        public string StateApp { get => activeRsaModel.StateApp; set { activeRsaModel.StateApp = value; OnPropertyChanged(); } }
        public string FileText { get => activeRsaModel.FileText; set { activeRsaModel.FileText = value; OnPropertyChanged(); } }
        public string ProgramText { get => activeRsaModel.ProgramText; set { activeRsaModel.ProgramText = value; OnPropertyChanged(); } }
        public Cryptography.Options Options { get => activeRsaModel.Options; set { activeRsaModel.Options = value;
                OnPropertyChanged(); if (activeRsaModel.State == State.FileText_Source) ProgramText = ""; else FileText = ""; } }
        public Database.Entities.Key? ActiveKey
        {
            get => activeRsaModel.ActiveKey;
            set
            {
                activeRsaModel.ActiveKey = value;
                if (activeRsaModel.ActiveKey != null)
                    activeRsaModel.Key = new Cryptography.Key
                        (activeRsaModel.ActiveKey.KeyE, activeRsaModel.ActiveKey.KeyN, activeRsaModel.ActiveKey.Bits);
                else
                    activeRsaModel.Key = new Cryptography.Key();
            }
        }
        public State State { get => activeRsaModel.State; set { activeRsaModel.State = value; OnPropertyChanged(); } }
        public KeysRepository Repository { get => activeRsaModel.Repository; }
        #endregion

        #region Создать файл
        private RelayCommand createFile;
        public RelayCommand CreateFile
        {
            get
            {
                return createFile ??= new RelayCommand(obj =>
                    {
                        SelectedPath = String.Empty;
                        FileText = String.Empty;
                        ProgramText = String.Empty;
                        State = State.FileText_Source;
                        CanEditProgText = false;
                        CanEditFileText = true;
                        StateApp = "[режим приложения: шифрование]";
                        IsEnabledEditButton = false;
                        IsEnabledDecryptButton = false;
                        IsEnabledEncryptButton = true;
                    });
            }
        }
        #endregion

        #region Открыть файл
        private RelayCommand openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ??= new RelayCommand(obj =>
                {
                    try
                    {
                        if (dialogService.OpenFileDialog() == true)
                        {
                            SelectedPath = dialogService.FilePath;
                            string text = String.Empty;
                            fileService.Open(SelectedPath, ref text, out bool isCrypto, out string opt, out string keyE, out string keyN);
                            if (isCrypto)
                            {
                                if (opt.Equals("SimpleRSA")) IsSimpleRSA = true;
                                else if (opt.Equals("RSAWhitAddBytes")) IsRSAAdd = true;
                                else if (opt.Equals("RSAComp")) IsRSAComp = true;
                                else if (opt.Equals("RSAEP_OAEP")) IsRSAEP_OAEP = true;
                                else isCrypto = false;
                                if (isCrypto)
                                {
                                    State = State.FileText_Crypto;
                                    var key = activeRsaModel.Repository.Get(keyE, keyN);
                                    activeRsaModel.Repository.SetActiveKey(key);
                                    ActiveKey = key;
                                    activeRsaModel.Key = new Cryptography.Key(key.Bits, key.KeyE, key.KeyN, key.KeyD);
                                    FileText = text;
                                    ProgramText = "";
                                    StateApp = "[режим приложения: дешифрование]";
                                    CanEditFileText = false;
                                    IsEnabledRadioButton = false;
                                    IsEnabledDecryptButton = true;      
                                    IsEnabledEncryptButton = false;
                                    IsEnabledManagerKeysButton = false;
                                }
                            }
                            if (!isCrypto)
                            {
                                State = State.FileText_Source; 
                                FileText = text; ProgramText = ""; 
                                StateApp = "[режим приложения: шифрование]";
                                CanEditFileText = true;
                                IsEnabledRadioButton = true;
                                IsEnabledDecryptButton = false;
                                IsEnabledEncryptButton = true;
                                IsEnabledManagerKeysButton = true;
                            }
                            CanEditProgText = false;
                            IsEnabledEditButton = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        dialogService.ShowMessage(ex.Message);
                    }
                });
            }
        }
        #endregion

        #region Открыть окно с ключами
        private RelayCommand openDialogWindow;
        public RelayCommand OpenDialogWindow
        {
            get
            {
                return openDialogWindow ??= new RelayCommand(obj =>
                {
                    var displayRootRegistry = ((App)Application.Current).displayRootRegistry;

                    childWindow.ShowWindow(displayRootRegistry.CreateWindowInstanceWithVM
                        (new ManagerKeysViewModel(activeRsaModel.Repository)));
                    var newKey = activeRsaModel.Repository.GetActiveKey();
                    if (newKey != ActiveKey)
                    {
                        if (activeRsaModel.State == State.FileText_Source)
                            ProgramText = "";
                        else FileText = "";
                        ActiveKey = newKey;
                    }
                });
            }
        }
        #endregion

        #region Шифрование
        private RelayCommand encrypt;
        public RelayCommand Encrypt
        {
            get
            {
                return encrypt ??= new RelayCommand(obj =>
                {
                    try
                    {
                        if (State == State.FileText_Source)
                            ProgramText = activeRsaModel.Encrypt(FileText);
                        else
                            FileText = activeRsaModel.Encrypt(ProgramText);
                    }
                    catch (Exception ex)
                    {
                        dialogService.ShowMessage(ex.Message);
                    }
                });
            }
        }
        #endregion

        #region Дешифрование
        private RelayCommand decrypt;
        public RelayCommand Decrypt
        {
            get
            {
                return decrypt ??= new RelayCommand(obj =>
                {
                    try
                    {                        
                        ProgramText = activeRsaModel.Decrypt(FileText);
                        if (CanEditProgText && !IsEnabledEditButton) 
                            IsEnabledEditButton = true;
                    }
                    catch (Exception ex)
                    {
                        dialogService.ShowMessage(ex.Message);
                    }
                });
            }
        }
        #endregion

        #region Редактировать текст 
        private RelayCommand editCommand;
        public RelayCommand EditCommand => editCommand ??= new RelayCommand(obj =>
                                                        {
                                                            CanEditProgText = true;
                                                            StateApp = "[режим приложения: шифрование и дешифрование]";
                                                            IsEnabledRadioButton = true;
                                                            IsEnabledEditButton = false;
                                                            IsEnabledEncryptButton = true;
                                                            IsEnabledManagerKeysButton = true;
                                                        });
        #endregion

        #region Сохранить файл
        private RelayCommand saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return saveCommand ??= new RelayCommand(obj =>
                  {
                      try
                      {
                          if (!String.IsNullOrEmpty(SelectedPath) || dialogService.SaveFileDialog() == true)
                          {
                              SelectedPath = (!String.IsNullOrEmpty(SelectedPath)) ? dialogService.FilePath : SelectedPath;
                              string opt;
                              if (Options == Cryptography.Options.SimpleRSA) opt = "SimpleRSA";
                              else if (Options == Cryptography.Options.RSA_withAddBytes) opt = "RSAWhitAddBytes";
                              else opt = (Options == Cryptography.Options.RSA_withCompBytes) ?
                                  "RSAComp" : "RSAEP_OAEP";
                              string text = (State == State.FileText_Source) ? ProgramText : FileText;
                              fileService.Save(SelectedPath, activeRsaModel.ActiveKey.KeyE, activeRsaModel.ActiveKey.KeyN, opt, text);
                              dialogService.ShowMessage("Файл сохранен!");
                          }
                      }
                      catch (Exception ex)
                      {
                          dialogService.ShowMessage(ex.Message);
                      }
                  });
            }
        }
        #endregion

        #region Сохранить шифротекст как...
        private RelayCommand saveCommandAs;
        public RelayCommand SaveCommandAs
        {
            get
            {
                return saveCommandAs ??= new RelayCommand(obj =>
                  {
                      try
                      {
                          if (dialogService.SaveFileDialog() == true)
                          {
                              SelectedPath = (!String.IsNullOrEmpty(SelectedPath)) ? dialogService.FilePath : SelectedPath;
                              string opt;
                              if (Options == Cryptography.Options.SimpleRSA) opt = "SimpleRSA";
                              else if (Options == Cryptography.Options.RSA_withAddBytes) opt = "RSAWhitAddBytes";
                              else opt = (Options == Cryptography.Options.RSA_withCompBytes) ?
                                  "RSAComp" : "RSAEP_OAEP";
                              string text = (State == State.FileText_Source) ? ProgramText : FileText;
                              fileService.Save(SelectedPath, activeRsaModel.ActiveKey.KeyE, activeRsaModel.ActiveKey.KeyN, opt, text);
                              dialogService.ShowMessage("Файл сохранен!");
                          }
                      }
                      catch (Exception ex)
                      {
                          dialogService.ShowMessage(ex.Message);
                      }
                  });
            }
        }
        #endregion

        #region Сохранить исходный текст как...
        private RelayCommand saveSourceCommand;
        public RelayCommand SaveSourceCommand
        {
            get
            {
                return saveSourceCommand ??= new RelayCommand(obj =>
                  {
                      try
                      {
                          if (dialogService.SaveFileDialog() == true)
                          {
                              string text = (State == State.FileText_Crypto) ? ProgramText : FileText;
                              fileService.Save(dialogService.FilePath, text);
                              dialogService.ShowMessage("Файл сохранен!");
                          }
                      }
                      catch (Exception ex)
                      {
                          dialogService.ShowMessage(ex.Message);
                      }
                  });
            }
        }
        #endregion

    }
}