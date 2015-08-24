using CheckMapp.Model.DataService;
using CheckMapp.Model.Tables;
using CheckMapp.Utils;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Controls;
using System;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using CheckMapp.Model.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Device.Location;
using Windows.Devices.Geolocation;
using FluentValidation;
using CheckMapp.Utils.Validations;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using CheckMapp.ViewModel;
using CheckMapp.Utils.EditableObject;

namespace CheckMapp.ViewModels.TripViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class AddEditTripViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        private Trip _trip;

        // Used for validate the form
        private IValidator<Trip> _validator;
        public List<string> _friendList;

        /// <summary>
        /// Initializes a new instance of the AddTripViewModel class.
        /// </summary>
        public AddEditTripViewModel(INavigationService navigationService)
        {

            Messenger.Default.Register<Tuple<int, Mode>>(this,
tuple =>
{
    this.Mode = tuple.Item2;
    Departure = String.Empty;
    Destination = String.Empty;

    if (this.Mode == Mode.add)
    {
        Trip = new Trip();
        Trip.BeginDate = DateTime.Now;
    }
    else
    {
        DataServiceTrip dsTrip = new DataServiceTrip();
        this.Trip = dsTrip.getTripById(tuple.Item1);
    }

    this.FriendList = Utility.FriendToList(Trip.FriendList);
    InitialiseValidator();

    EditableObject = new Caretaker<Trip>(this.Trip);
    EditableObject.BeginEdit();
});

            this._navigationService = navigationService;

            
        }

        private void InitialiseValidator()
        {
            _validator = new ValidatorFactory().GetValidator<Trip>();
        }

        /// <summary>
        /// Mon objet editable, nécessaire pour annuler les changements
        /// </summary>
        private Caretaker<Trip> EditableObject { get; set; }

        private ICommand _addEditTripCommand;
        /// <summary>
        /// Commande pour ajouter un voyage
        /// </summary>
        public ICommand AddEditTripCommand
        {
            get
            {
                if (_addEditTripCommand == null)
                {
                    _addEditTripCommand = new RelayCommand(() => AddEditTrip());
                }
                return _addEditTripCommand;
            }

        }

        private ICommand _goBackCommand;

        public ICommand GoBackCommand
        {
            get
            {
                if (_goBackCommand == null)
                {
                    _goBackCommand = new RelayCommand(() => GoBackNav());
                }
                return _goBackCommand;
            }
        }

        private void GoBackNav()
        {
            if (Mode != Mode.add)
            {
                Messenger.Default.Send<int, TripViewModel>(Trip.Id);
            }
            _navigationService.GoBack();
        }

        private ICommand _mainPageCommand;

        public ICommand MainPageCommand
        {
            get
            {
                if (_mainPageCommand == null)
                {
                    _mainPageCommand = new RelayCommand(() => MainPageNav());
                }
                return _mainPageCommand;
            }
        }

        private void MainPageNav()
        {
            Messenger.Default.Send<string, MainViewModel>("");
            _navigationService.NavigateTo("MainView");
        }

        #region Properties

        private bool _isFormValid;

        public bool IsFormValid
        {
            get { return _isFormValid; }
            set
            {
                _isFormValid = value;
            }
        }

        public Trip Trip
        {
            get { return _trip; }
            set
            {
                _trip = value;
            }
        }

        public Mode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// Nom du voyage
        /// </summary>
        public string TripName
        {
            get { return Trip.Name; }
            set
            {
                if (Trip.Name != value)
                {
                    Trip.Name = value;
                    RaisePropertyChanged("TripName");
                }
            }
        }

        /// <summary>
        /// Liste d'amis
        /// </summary>
        public List<string> FriendList
        {
            get { return _friendList; }
            set
            {
                if (_friendList != value)
                {
                    _friendList = value;
                    RaisePropertyChanged("FriendList");
                }
            }
        }

        /// <summary>
        /// Date de départ
        /// </summary>
        public DateTime TripBeginDate
        {
            get { return Trip.BeginDate; }
            set
            {
                if (!Trip.BeginDate.Equals(value))
                {
                    Trip.BeginDate = value;
                    RaisePropertyChanged("TripBeginDate");

                }
            }
        }

        /// <summary>
        /// Date de fin
        /// </summary>
        public DateTime? TripEndDate
        {
            get { return Trip.EndDate; }
            set
            {
                if (!Trip.EndDate.Equals(value))
                {
                    Trip.EndDate = value;
                    RaisePropertyChanged("TripEndDate");
                }
            }
        }

        /// <summary>
        /// Nom du départ
        /// </summary>
        private string _departure;
        public string Departure
        {
            get { return _departure; }
            set
            {
                if (_departure != value)
                {
                    _departure = value;
                    RaisePropertyChanged("Departure");
                }
            }
        }

        /// <summary>
        /// Nom de la destination
        /// </summary>
        private string _destination;
        public string Destination
        {
            get { return _destination; }
            set
            {
                if (_destination != value)
                {
                    _destination = value;
                    RaisePropertyChanged("Destination");
                }
            }
        }

        /// <summary>
        /// Coordonnées départ longitude
        /// </summary>
        public double DepartureLongitude
        {
            get { return Trip.DepartureLongitude; }
            set
            {
                if (Trip.DepartureLongitude != value)
                {
                    Trip.DepartureLongitude = value;
                    RaisePropertyChanged("DepartureLongitude");
                }
            }
        }

        /// <summary>
        /// Coordonnées départ latitude
        /// </summary>
        public double DepartureLatitude
        {
            get { return Trip.DepartureLatitude; }
            set
            {
                if (Trip.DepartureLatitude != value)
                {
                    Trip.DepartureLatitude = value;
                    RaisePropertyChanged("DepartureLatitude");
                }
            }
        }

        /// <summary>
        /// Coordonnées destination longitude
        /// </summary>
        public double DestinationLongitude
        {
            get { return Trip.DestinationLongitude; }
            set
            {
                if (Trip.DestinationLongitude != value)
                {
                    Trip.DestinationLongitude = value;
                    RaisePropertyChanged("DestinationLongitude");
                }
            }
        }

        /// <summary>
        /// Coordonnées destination latitude
        /// </summary>
        public double DestinationLatitude
        {
            get { return Trip.DestinationLatitude; }
            set
            {
                if (Trip.DestinationLatitude != value)
                {
                    Trip.DestinationLatitude = value;
                    RaisePropertyChanged("DestinationLatitude");
                }
            }
        }

        /// <summary>
        /// Image principale
        /// </summary>
        public byte[] MainImage
        {
            get { return Trip.MainPictureData; }
            set
            {
                Trip.MainPictureData = value;
                RaisePropertyChanged("MainImage");
            }
        }


        private ICommand _cancelTripCommand;
        public ICommand CancelTripCommand
        {
            get
            {
                if (_cancelTripCommand == null)
                {
                    _cancelTripCommand = new RelayCommand(() => CancelTrip());
                }
                return _cancelTripCommand;
            }
        }

        #endregion

        #region DBMethods

        public void AddEditTrip()
        {
            // If the form is not valid, a notification will appear
            if (ValidationErrorsHandler.IsValid(_validator, Trip))
            {
                _isFormValid = true;
                Trip.FriendList = Utils.Utility.FriendToString(FriendList);
                // Adding a Trip
                if (Mode == Mode.add)
                {
                    AddTripInDB();
                    Messenger.Default.Send<string, MainViewModel>("");
                }
                else if (Mode == Mode.edit)
                {
                    UpdateExistingTrip();
                }

                EditableObject.EndEdit();
            }
            else
            {
                _isFormValid = false;
            }
        }

        public void CancelTrip()
        {
            EditableObject.CancelEdit();
        }

        public void AddTripInDB()
        {
            DataServiceTrip dsTrip = new DataServiceTrip();
            dsTrip.addTrip(Trip);
        }

        public void UpdateExistingTrip()
        {
            DataServiceTrip dsTrip = new DataServiceTrip();
            dsTrip.UpdateTrip(Trip);
        }

        #endregion

    }
}