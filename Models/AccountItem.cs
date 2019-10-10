using DevExpress.Mvvm;
using MMS.Domain;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace MMS.Models
{
    public class AccountItem : INotifyPropertyChanged
    {
        [JsonProperty]
        public Platform SelectedPlatform {
            get;
            private set;
        }
        
        public string PlatformCode
        {
            get { return SelectedPlatform.ToString().Substring(0, 1); }
        }

        [JsonProperty]
        public string ApiKey
        {
            get;
            private set;
        }

        [JsonProperty]
        public string ApiSecret
        {
            get;
            private set;
        }

        [JsonIgnore]
        public bool Delete {
            get { return _delete; }
            set
            {
                this.MutateVerbose(ref _delete, value, RaisePropertyChanged());
            }
        }
        [JsonIgnore]
        private bool _delete;
        
        public event PropertyChangedEventHandler PropertyChanged;

        public AccountItem(
            Platform selectedPlatforms,
            string apiKey,
            string apiSecret
            )
        {
            SelectedPlatform = selectedPlatforms;
            ApiSecret = apiSecret;
            ApiKey = apiKey;
            Delete = false;
        }

        
        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }

        [JsonIgnore]
        public ICommand DeleteCommand {
            get {
                return new DelegateCommand(()=> {
                    Delete = true;
                });
            }
        }
    }
}
