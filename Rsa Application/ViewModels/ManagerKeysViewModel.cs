using Rsa_Application.Common.Abstractions;
using Rsa_Application.Common.Services;
using Rsa_Application.Database;
using Rsa_Application.Infrastructure.Interfaces;
using Rsa_Application.Models;
using System.Collections.ObjectModel;
using System.Windows;
using Key = Rsa_Application.Database.Entities.Key;

namespace Rsa_Application.ViewModels
{
    internal class ManagerKeysViewModel : ViewModel
    {
        private readonly ManagerKeysModel managerKeys;

        readonly IWindowService childWindow;

        public ManagerKeysViewModel(KeysRepository keys)
        {
            managerKeys = new();
            Repository = keys;
            try { Keys = new ObservableCollection<Key>(keys.GetAll()); }
            catch { Keys = new(); }
            childWindow = new WindowService();
        }

        public ObservableCollection<Key> Keys { get => managerKeys.Keys; set { managerKeys.Keys = value; OnPropertyChanged(); } }
        public Key SelectedKey { get => managerKeys.SelectedKey; set { managerKeys.SelectedKey = value; OnPropertyChanged(); } }
        public KeysRepository Repository { get => managerKeys.Repository; set { managerKeys.Repository = value; OnPropertyChanged(); } }

        #region Открыть окно с ключами
        private RelayCommand openDialogWindow;
        public RelayCommand OpenDialogWindow
        {
            get
            {
                return openDialogWindow ??= new RelayCommand(obj =>
                {
                    var displayRootRegistry = (Application.Current as App).displayRootRegistry;

                    childWindow.ShowWindow(displayRootRegistry.CreateWindowInstanceWithVM
                        (new AddKeyViewModel(Repository, Keys.Count)));
                });
            }
        }
        #endregion

        #region Обновление ключей
        private RelayCommand update;
        public RelayCommand Update
        {
            get
            {
                return update ??= new RelayCommand(obj =>
                {
                    Keys = new ObservableCollection<Key>(Repository.GetAll());
                });
            }
        }
        #endregion

        #region Удаление ключа
        private RelayCommand deleteKey;
        public RelayCommand DeleteKey
        {
            get
            {
                return deleteKey ??= new RelayCommand(obj =>
                {
                    if (SelectedKey != null)
                    {
                        Repository.Delete(SelectedKey.Id);
                        SelectedKey = null;
                        Keys = new ObservableCollection<Key>(Repository.GetAll());
                    }
                });
            }
        }
        #endregion

        #region Изменение статуса ключей
        private RelayCommand setActive;
        public RelayCommand SetActive
        {
            get
            {
                return setActive ??= new RelayCommand(obj =>
                {
                    if (SelectedKey != null)
                    {
                        Repository.SetActiveKey(SelectedKey);
                        Keys = new ObservableCollection<Key>(Repository.GetAll());
                    }
                });
            }
        }
        #endregion
    }
}