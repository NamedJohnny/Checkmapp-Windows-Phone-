using CheckMapp.Model.DataService;
using CheckMapp.ViewModels;
using CheckMapp.ViewModels.TripViewModels;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CheckMapp.ViewModel
{
    public class DashboardViewModel
    {
        private INavigationService _navigationService;
        public DashboardViewModel(INavigationService navigationService)
        {
            this._navigationService = navigationService;
        }

        private ICommand _mapCommand;

        public ICommand MapCommand
        {
            get
            {
                if (_mapCommand == null)
                {
                    _mapCommand = new RelayCommand(() => MapNav());
                }
                return _mapCommand;
            }
        }

        private void MapNav()
        {
            Messenger.Default.Send<string, MapViewModel>("");
            _navigationService.NavigateTo("MapView");
        }

        private ICommand _tripCommand;

        public ICommand TripCommand
        {
            get
            {
                if (_tripCommand == null)
                {
                    _tripCommand = new RelayCommand(() => TripNav());
                }
                return _tripCommand;
            }
        }

        private void TripNav()
        {
            DataServiceTrip dsTrip = new DataServiceTrip();
            TripList = dsTrip.LoadTrip();
            Messenger.Default.Send<int, TripViewModel>(TripList.Find(x => x.IsActif).Id);
            _navigationService.NavigateTo("TripView");
        }

        private ICommand _addEditTripCommand;

        public ICommand AddEditTripCommand
        {
            get
            {
                if (_addEditTripCommand == null)
                {
                    _addEditTripCommand = new RelayCommand(() => AddTripNav());
                }
                return _addEditTripCommand;
            }
        }

        private void AddTripNav()
        {
            Messenger.Default.Send<Tuple<int, Mode>, AddEditTripViewModel>(new Tuple<int, Mode>(0,Mode.add));
            _navigationService.NavigateTo("AddEditTripView");
        }

        private ICommand _settingsCommand;

        public ICommand SettingsCommand
        {
            get
            {
                if (_settingsCommand == null)
                {
                    _settingsCommand = new RelayCommand(() => _navigationService.NavigateTo("SettingsView"));
                }
                return _settingsCommand;
            }
        }




        public List<Model.Tables.Trip> TripList { get; set; }
    }
}
