using CheckMapp.Model;
using CheckMapp.Resources;
using GalaSoft.MvvmLight;
using System;
using CheckMapp.Model.Tables;
using CheckMapp.Model.DataService;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.ComponentModel;
using GalaSoft.MvvmLight.Views;
using GalaSoft.MvvmLight.Messaging;
using CheckMapp.ViewModels.PhotoViewModels;
using CheckMapp.ViewModels.POIViewModels;
using CheckMapp.ViewModels.NoteViewModels;
using CheckMapp.ViewModel;

namespace CheckMapp.ViewModels.TripViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class TripViewModel : ViewModelBase
    {
        private Trip _trip;
        private INavigationService _navigationService;
        /// <summary>
        /// Initializes a new instance of the TripViewModel class.
        /// </summary>
        public TripViewModel(INavigationService navigationService)
        {
            Messenger.Default.Register<int>(this,
trip =>
{
    DataServiceTrip dsTrip = new DataServiceTrip();
    this.Trip = dsTrip.getTripById(trip);

    RaisePropertyChanged("NoteTitle");
    RaisePropertyChanged("PhotoTitle");
    RaisePropertyChanged("POITitle");
});
            this._navigationService = navigationService;

        }

        public Trip Trip
        {
            get { return _trip; }
            set
            {
                _trip = value;
            }
        }

        /// <summary>
        /// Le titre des notes dans le voyage
        /// </summary>
        public string NoteTitle
        {
            get
            {
                if (Trip.Notes != null)
                {
                    return String.Format(AppResources.NoteTripTitle, Trip.Notes.Count);
                }
                else
                {
                    return String.Format(AppResources.NoteTripTitle, 0);
                }
            }
        }

        /// <summary>
        /// Le titre des photos dans le voyage
        /// </summary>
        public string PhotoTitle
        {
            get
            {
                if (Trip.Pictures != null)
                {
                    return String.Format(AppResources.PhotoTripTitle, Trip.Pictures.Count);
                }
                else
                {
                    return String.Format(AppResources.PhotoTripTitle, 0);
                }
            }
        }

        /// <summary>
        /// Le titre des points d'intérets dans le voyage
        /// </summary>
        public string POITitle
        {
            get
            {
                if (Trip.PointsOfInterests != null)
                {
                    DataServicePoi dsPoi = new DataServicePoi();
                    return String.Format(AppResources.POITripTitle, dsPoi.LoadPointOfInterestsFromTrip(Trip).Count);
                }
                else
                {
                    return String.Format(AppResources.POITripTitle, 0);
                }
            }
        }

        #region Buttons

        private ICommand _deleteTripCommand;
        public ICommand DeleteTripCommand
        {
            get
            {
                if (_deleteTripCommand == null)
                {
                    _deleteTripCommand = new RelayCommand(() => DeleteTrip());
                }
                return _deleteTripCommand;
            }

        }

        private ICommand _listPhotoCommand;

        public ICommand ListPhotoCommand
        {
            get
            {
                if (_listPhotoCommand == null)
                {
                    _listPhotoCommand = new RelayCommand(() => ListPhotoNav());
                }
                return _listPhotoCommand;
            }
        }

        private void ListPhotoNav()
        {
            Messenger.Default.Send<int, ListPhotoViewModel>(Trip.Id);
            _navigationService.NavigateTo("ListPhotoView");
        }

        private ICommand _listNoteCommand;

        public ICommand ListNoteCommand
        {
            get
            {
                if (_listNoteCommand == null)
                {
                    _listNoteCommand = new RelayCommand(() => ListNoteNav());
                }
                return _listNoteCommand;
            }
        }

        private void ListNoteNav()
        {
            Messenger.Default.Send<int, ListNoteViewModel>(Trip.Id);
            _navigationService.NavigateTo("ListNoteView");
        }

        private ICommand _listPOICommand;

        public ICommand ListPOICommand
        {
            get
            {
                if (_listPOICommand == null)
                {
                    _listPOICommand = new RelayCommand(() => ListPOINav());
                }
                return _listPOICommand;
            }
        }

        private void ListPOINav()
        {
            Messenger.Default.Send<int, ListPOIViewModel>(Trip.Id);
            _navigationService.NavigateTo("ListPOIView");
        }

        private ICommand _statsCommand;

        public ICommand StatsCommand
        {
            get
            {
                if (_statsCommand == null)
                {
                    _statsCommand = new RelayCommand(() => StatsNav());
                }
                return _statsCommand;
            }
        }

        private void StatsNav()
        {
            Messenger.Default.Send<int, StatisticViewModel>(Trip.Id);
            _navigationService.NavigateTo("StatisticView");
        }

        private ICommand _addNoteCommand;

        public ICommand AddNoteCommand
        {
            get
            {
                if (_addNoteCommand == null)
                {
                    _addNoteCommand = new RelayCommand<Tuple<int, Mode>>((tuple) => AddNoteNav(tuple));
                }
                return _addNoteCommand;
            }
        }

        private void AddNoteNav(Tuple<int, Mode> tuple)
        {
            Messenger.Default.Send<Tuple<int, int, Mode>, AddEditNoteViewModel>(new Tuple<int, int, Mode>(Trip.Id, tuple.Item1, tuple.Item2));
            _navigationService.NavigateTo("AddEditNoteView");
        }

        private ICommand _addPhotoCommand;

        public ICommand AddPhotoCommand
        {
            get
            {
                if (_addPhotoCommand == null)
                {
                    _addPhotoCommand = new RelayCommand<Tuple<int, Mode>>((tuple) => AddPhotoNav(tuple));
                }
                return _addPhotoCommand;
            }
        }

        private void AddPhotoNav(Tuple<int, Mode> tuple)
        {
            Messenger.Default.Send<Tuple<int, int, Mode>, AddEditPhotoViewModel>(new Tuple<int, int, Mode>(Trip.Id, tuple.Item1, tuple.Item2));
            _navigationService.NavigateTo("AddEditPhotoView");
        }

        private ICommand _addPOICommand;

        public ICommand AddPOICommand
        {
            get
            {
                if (_addPOICommand == null)
                {
                    _addPOICommand = new RelayCommand<Tuple<int, Mode>>((tuple) => AddPOINav(tuple));
                }
                return _addPOICommand;
            }
        }

        private void AddPOINav(Tuple<int, Mode> tuple)
        {
            Messenger.Default.Send<Tuple<int, int, Mode>, AddEditPOIViewModel>(new Tuple<int, int, Mode>(Trip.Id, tuple.Item1, tuple.Item2));
            _navigationService.NavigateTo("AddEditPOIView");
        }

        private ICommand _editTripCommand;

        public ICommand EditTripCommand
        {
            get
            {
                if (_editTripCommand == null)
                {
                    _editTripCommand = new RelayCommand(() => EditTripNav());
                }
                return _editTripCommand;
            }
        }

        private void EditTripNav()
        {
            Messenger.Default.Send<Tuple<int, Mode>, AddEditTripViewModel>(new Tuple<int, Mode>(Trip.Id, Mode.edit));
            _navigationService.NavigateTo("AddEditTripView");
        }

        private ICommand _selectEndDateCommand;

        public ICommand SelectEndDateCommand
        {
            get
            {
                if (_selectEndDateCommand == null)
                {
                    _selectEndDateCommand = new RelayCommand(() => SelectEndDateNav());
                }
                return _selectEndDateCommand;
            }
        }

        private void SelectEndDateNav()
        {
            Messenger.Default.Send<int, SelectEndDateViewModel>(Trip.Id);
            _navigationService.NavigateTo("SelectEndDateView");
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

        #endregion

        #region DBMethods

        public void DeleteTrip()
        {
            DataServiceTrip dsTrip = new DataServiceTrip();
            dsTrip.DeleteTrip(Trip);
        }


        #endregion

    }
}