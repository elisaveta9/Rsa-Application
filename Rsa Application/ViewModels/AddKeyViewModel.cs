using Rsa_Application.Common.Abstractions;
using Rsa_Application.Common.Services;
using Rsa_Application.Database;
using Rsa_Application.Database.Entities;
using Rsa_Application.Infrastructure.Interfaces;
using Rsa_Application.Models;
using System;
using System.Collections.Generic;

namespace Rsa_Application.ViewModels
{
    internal class AddKeyViewModel : ViewModel
    {
        private readonly AddKeyModel model = new();

        readonly IDialogService dialogService;

        public AddKeyViewModel(KeysRepository keys, int count)
        {
            Repository = keys;
            dialogService = new DefaultDialogService();
            CountKeys = count;
        }

        #region Свойства
        public KeysRepository Repository { get => model.Repository; set { model.Repository = value; OnPropertyChanged(); } }
        public List<int> BBits { get => model.BBits; }
        public KLen SelectedPathLenght { get => model.SelectedPathLenght; set { model.SelectedPathLenght = value; OnPropertyChanged(); } }
        public string? Name { get => model.Name; set { model.Name = value; OnPropertyChanged("name"); } }       
        public string LowBits
        {
            get => $"{model.LowBits}";
            set
            {
                model.LowBits = int.Parse(value);
                if (SelectedPathLenght == KLen.lLen)
                    Bits = model.LowBits;
                OnPropertyChanged();
            }
        }
        public int BigBits 
        { 
            get => model.BigBits; 
            set 
            { 
                model.BigBits = value;
                if (SelectedPathLenght == KLen.bLen)
                    Bits = value;
                OnPropertyChanged(); 
            } 
        }
        public int CountKeys { get => model.CountKeys; set { model.CountKeys = value; } }
        public int Bits { get => model.Bits; set { model.Bits = value; OnPropertyChanged(); } }
        public bool IsSelectLowBits 
        {
            get => SelectedPathLenght == KLen.lLen;
            set
            {
                if (value)
                {
                    SelectedPathLenght = KLen.lLen;
                    Bits = model.LowBits;
                }
                else SelectedPathLenght = KLen.bLen;
            }
        }
        public bool IsSelectBigBits
        {
            get => SelectedPathLenght == KLen.bLen;
            set
            {
                if (value)
                {
                    SelectedPathLenght = KLen.bLen;
                    Bits = model.BigBits;
                }
                else SelectedPathLenght = KLen.lLen;
            }
        }
        #endregion

        #region Добавление ключа в базу данных
        private RelayCommand addKey;
        public RelayCommand AddKey
        {
            get
            {
                return addKey ??= new RelayCommand(obj =>
                {
                    if (String.IsNullOrEmpty(model.Name))
                        dialogService.ShowMessage("Не задано имя ключа!");
                    if (model.Bits.IsNotNull())
                    {
                        try
                        {
                            string state = (CountKeys == 0) ? "активен" : "неактивен";
                            Key key = new(Name, null, state, Bits, null, null, null);
                            Repository.Add(key);
                            dialogService.ShowMessage("Ключ создан и добавлен!");
                        }
                        catch (Exception ex)
                        { dialogService.ShowMessage(ex.Message); }
                    }
                    else dialogService.ShowMessage("Не задано количество бит!");                    
                });
            }
        } 
        #endregion
    }
}