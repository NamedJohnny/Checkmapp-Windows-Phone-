using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Controls;
using System.ComponentModel;
using CheckMapp.Utils.Settings;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using CheckMapp.Utils.Languages;
using Utility = CheckMapp.Utils.Utility;
using CheckMapp.Resources;
using System.Windows;

namespace CheckMapp.ViewModels.SettingsViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private CMIsolatedStorageProperty<bool> WifiOnlyStorageProperty;
        private CMIsolatedStorageProperty<string> LanguageStorageProperty;

        private List<string> _languagesCode;

        public SettingsViewModel()
        {
            _languagesCode = new List<string>();
            loadData();
            Loading = false;
        }

        private void loadData()
        {
            _appVersion = Utility.GetAppVersion();
            _languagesList = LocalizationManager.GetAllLanguages();
            _languagesCode = LocalizationManager.GetAllLanguagesCode();
            loadDataFromIsolatedStorage();
        }

        private void loadDataFromIsolatedStorage()
        {
            WifiOnlyStorageProperty = CMSettingsContainer.WifiOnly;
            _wifiOnly = WifiOnlyStorageProperty.Value;

            LanguageStorageProperty = CMSettingsContainer.Language;

            SetLanguageListPicker();
        }

        private void SetLanguageListPicker()
        {
            int index;

            if (LanguageStorageProperty.Value != null)
                index = Enumerable.Range(0, _languagesCode.Count)
                    .Where(x => _languagesCode[x] == LanguageStorageProperty.Value).FirstOrDefault();
            else
            {
                string currentLang = LocalizationManager.GetCurrentAppLang();

                index = Enumerable.Range(0, _languagesCode.Count)
                    .Where(x => _languagesCode[x] == currentLang).FirstOrDefault();

                LanguageStorageProperty.Value = currentLang;
            }

            if (index > -1)
                LanguageIndex = index;
        }

        #region Properties

        private string _appVersion;

        public string AppVersion
        {
            get { return _appVersion; }
            set
            {
                _appVersion = value;
                RaisePropertyChanged("AppVersion");
            }
        }

        private bool _wifiOnly;

        public bool WifiOnly
        {
            get { return _wifiOnly; }
            set
            {
                _wifiOnly = value;
                WifiOnlyStorageProperty.Value = _wifiOnly;
                RaisePropertyChanged("WifiOnly");
            }
        }

        private int _languageIndex;

        public int LanguageIndex
        {
            get { return _languageIndex; }
            set
            {
                _languageIndex = value;
                UpdateLanguage();
                RaisePropertyChanged("LanguageIndex");
            }
        }

        private List<string> _languagesList;

        public List<string> LanguagesList
        {
            get { return _languagesList; }
            set
            {
                _languagesList = value;
                RaisePropertyChanged("LanguagesList");
            }
        }

        private bool _loading;

        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged("Loading");
            }
        }

        private string _typeProgress;

        public string TypeProgress
        {
            get { return _typeProgress; }
            set
            {
                _typeProgress = value;
                RaisePropertyChanged("TypeProgress");
            }
        }

        public string ProgressText
        {
            get { return ProgressPercent.ToString("0,0") + "%"; }
        }

        private double _progressPercent;
        public double ProgressPercent
        {
            get { return _progressPercent; }
            set
            {
                _progressPercent = value;
                RaisePropertyChanged("ProgressPercent");
                RaisePropertyChanged("ProgressText");
            }
        }

        #endregion

        #region Buttons Command

        private ICommand _importCommand;
        public ICommand ImportCommand
        {
            get
            {
                if (_importCommand == null)
                {
                    _importCommand = new RelayCommand(() => Import());
                }
                return _importCommand;
            }
        }


        private ICommand _exportCommand;
        public ICommand ExportCommand
        {
            get
            {
                if (_exportCommand == null)
                {
                    _exportCommand = new RelayCommand(() => Export());
                }
                return _exportCommand;
            }
        }

        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand(() => Cancel());
                }
                return _cancelCommand;
            }
        }


        public bool ImportInProgress { get; set; }
        public bool ExportInProgress { get; set; }


        #endregion


        #region Storage Methods

        /// <summary>
        /// Update the Language if it was not the same
        /// </summary>
        private void UpdateLanguage()
        {
            string newLang = _languagesCode[_languageIndex];

            if (!LanguageStorageProperty.Value.Equals(newLang))
            {
                LanguageStorageProperty.Value = newLang;
                MessageBox.Show(AppResources.RestartApp, "Information", MessageBoxButton.OK);
                Application.Current.Terminate();
            }
        }

        #endregion

        #region Buttons Methods
        private void Export()
        {
            this.Loading = true;
            this.TypeProgress = AppResources.ExportLoading;
            this.ExportInProgress = true;
        }


        private void Import()
        {
            this.Loading = true;
            this.TypeProgress = AppResources.ImportLoading;
            this.ImportInProgress = true;
        }

        private void Cancel()
        {
            this.Loading = false;
            ProgressPercent = 0;
            this.ImportInProgress = false;
            this.ExportInProgress = false;
        }

        #endregion
    }
}
