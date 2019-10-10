using MMS.Domain;
using MMS.Models;
using MMS.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MMS.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        public static ObservableCollection<AccountItem> AccountItems => _accountItems;
        private static ObservableCollection<AccountItem> _accountItems;

        private Platform _validatePlatform;
        public Platform ValidatePlatform
        {
            get { return _validatePlatform; }
            set
            {
                this.MutateVerbose(ref _validatePlatform, value, RaisePropertyChanged());
            }
        }

        public string ApiKeyValidate
        {
            get { return _apiKeyValidate; }
            set { this.MutateVerbose(ref _apiKeyValidate, value, RaisePropertyChanged()); }
        }
        public string SecretkeyValidate
        {
            get { return _secretkeyValidate; }
            set { this.MutateVerbose(ref _secretkeyValidate, value, RaisePropertyChanged()); }
        }
        public Platform PlatformValidate
        {
            get { return _platformValidate; }
            set { this.MutateVerbose(ref _platformValidate, value, RaisePropertyChanged()); }
        }

        private string _apiKeyValidate;
        private string _secretkeyValidate;
        private Platform _platformValidate;


        public IList<Enum> PlatformList { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        

        public HomeViewModel()
        {
            ValidatePlatform = Platform.HitBTC;
            PlatformList = new List<Enum>()
            {
                Platform.HitBTC,
                Platform.Liquid,
                Platform.Bistox
            };
            _accountItems = new ObservableCollection<AccountItem>();
            _accountItems = Settings.Default.EncryptedAccounts == ""
                ? new ObservableCollection<AccountItem>()
                : JsonConvert.DeserializeObject<ObservableCollection<AccountItem>>(Encrypter.Decrypt(Settings.Default.EncryptedAccounts, "136242sd"));

            foreach (var item in _accountItems) {
                item.PropertyChanged += (s, e) => {
                    if (e.PropertyName == "Delete")
                        _accountItems.Remove((s as AccountItem));
                    Settings.Default.EncryptedAccounts = Encrypter.Encrypt(JsonConvert.SerializeObject(AccountItems), "136242sd");
                    Settings.Default.Save();
                };
            }
        }

        public void AddNewAccount(string apiKey, string apiSecret, Platform selectedPlatform)
        {
            var newAccount = new AccountItem(selectedPlatform, apiKey, apiSecret);
            newAccount.PropertyChanged += (s, e) => {
                if (e.PropertyName == "Delete")
                    _accountItems.Remove((s as AccountItem));
                Settings.Default.EncryptedAccounts = Encrypter.Encrypt(JsonConvert.SerializeObject(AccountItems), "136242sd");
                Settings.Default.Save();
            };
            _accountItems.Add(newAccount);
            Settings.Default.EncryptedAccounts = Encrypter.Encrypt(JsonConvert.SerializeObject(AccountItems), "136242sd");
            Settings.Default.Save();
        }

        public void DeleteOldAccount(AccountItem account)
        {
            _accountItems.Remove((account));
            Settings.Default.EncryptedAccounts = Encrypter.Encrypt(JsonConvert.SerializeObject(AccountItems), "136242sd");
            Settings.Default.Save();
        }

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
