using CheckMapp.Model.DataService;
using CheckMapp.Model.Tables;
using CheckMapp.ViewModels;
using CheckMapp.ViewModels.ArchivesViewModels;
using CheckMapp.ViewModels.TripViewModels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace CheckMapp.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private ICommand _ShowUserControlTimelineCommand;
        private ICommand _ShowUserControlTripCommand;
        private INavigationService _navigationService;
        private ViewModelBase _currentPageViewModel;

        private bool _isList;
        private bool _isTimeline;

        public List<Trip> TripList { get; set; }
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(INavigationService navigationService)
        {
            Messenger.Default.Register<string>(this,
useless =>
{
    Init();
});
            Init();
            this._navigationService = navigationService;
        }

        private void Init()
        {
            LoadArchivesTripFromDatabase();
            PageViewModels = new List<ViewModelBase>();
            // Add available pages
            Messenger.Default.Send<List<Trip>, ArchivesViewModel>(TripListActif());
            Messenger.Default.Send<List<Trip>, TimelineViewModel>(TripListActif());
            Messenger.Default.Send<string, MapViewModel>("");
            // Set starting page
            ShowUserControlTrip();

            TripActif = TripList.Find(x => x.IsActif);
        }

        public void LoadArchivesTripFromDatabase()
        {
            DataServiceTrip dsTrip = new DataServiceTrip();
            TripList = dsTrip.LoadTrip();
        }

        public List<Trip> TripListActif()
        {
            return TripList.FindAll(x => !x.IsActif);
        }

        public Trip _tripActif;
        public Trip TripActif
        {
            get
            {
                return _tripActif;
            }
            set
            {
                _tripActif = value;
                RaisePropertyChanged("IsTripActif");
            }
        }

        public bool IsTripActif
        {
            get { return TripActif != null; }
        }

        /// <summary>
        /// Si c'est affiché en tableau
        /// </summary>
        public bool IsList
        {
            get { return _isList; }
            set
            {
                _isList = value;
                RaisePropertyChanged("IsList");
            }
        }

        /// <summary>
        /// Si c'est affiché en ligne de temps
        /// </summary>
        public bool IsTimeline
        {
            get { return _isTimeline; }
            set
            {
                _isTimeline = value;
                RaisePropertyChanged("IsTimeline");
            }
        }

        public ICommand ShowUserControlTripCommand
        {
            get
            {
                if (_ShowUserControlTripCommand == null)
                {
                    _ShowUserControlTripCommand = new RelayCommand(() => ShowUserControlTrip());
                }
                return _ShowUserControlTripCommand;
            }
        }

        public ICommand ShowUserControlTimelineCommand
        {
            get
            {
                if (_ShowUserControlTimelineCommand == null)
                {
                    _ShowUserControlTimelineCommand = new RelayCommand(() => ShowUserControlTimeline());
                }
                return _ShowUserControlTimelineCommand;
            }
        }


        /// <summary>
        /// Afficher en tableau
        /// </summary>
        public void ShowUserControlTrip()
        {
            CurrentPageViewModel = ServiceLocator.Current.GetInstance<ArchivesViewModel>();
            IsTimeline = false;
            IsList = true;
        }

        /// <summary>
        /// Afficher en ligne de temps
        /// </summary>
        public void ShowUserControlTimeline()
        {
            CurrentPageViewModel = ServiceLocator.Current.GetInstance<TimelineViewModel>();
            IsTimeline = true;
            IsList = false;
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



        public List<ViewModelBase> PageViewModels
        {
            get;
            set;
        }

        public ViewModelBase CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    RaisePropertyChanged("CurrentPageViewModel");
                }
            }
        }

        private ICommand _currentViewCommand;

        public ICommand CurrentViewCommand
        {
            get
            {
                if (_currentViewCommand == null)
                {
                    _currentViewCommand = new RelayCommand(() => CurrentViewNav());
                }
                return _currentViewCommand;
            }
        }

        private void CurrentViewNav()
        {
            if (TripActif != null)
                Messenger.Default.Send<int, CurrentViewModel>(TripActif.Id);
        }







    }
}