using CheckMapp.Model.DataService;
using CheckMapp.Model.Tables;
using CheckMapp.ViewModels.TripViewModels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Windows.Input;

namespace CheckMapp.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MapViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        private ObservableCollection<Trip> tripPoints;

        /// <summary>
        /// Initializes a new instance of the MapViewModel class.
        /// </summary>
        public MapViewModel(INavigationService navigationService)
        {
            Messenger.Default.Register<string>(this,
useless =>
{
    tripPoints = new ObservableCollection<Trip>();
    LoadAllCoordinateFromDatabase();
});
            
            this._navigationService = navigationService;
        }

        public ObservableCollection<Trip> TripPoints
        {
            get { return tripPoints; }
            set 
            { 
                tripPoints = value;
                RaisePropertyChanged("TripPoints");
            }
        }

        public void LoadAllCoordinateFromDatabase()
        {
            DataServiceTrip dsTrip = new DataServiceTrip();
            List<Trip> allTripInDB = dsTrip.LoadTrip();
            TripPoints = new ObservableCollection<Trip>(allTripInDB);
        }

        private ICommand _tripCommand;

        public ICommand TripCommand
        {
            get
            {
                if (_tripCommand == null)
                {
                    _tripCommand = new RelayCommand<int>((trip) => TripNav(trip));
                }
                return _tripCommand;
            }
        }

        private void TripNav(int trip)
        {
            Messenger.Default.Send<int, TripViewModel>(trip);
            _navigationService.NavigateTo("TripView");
        }
    }
}