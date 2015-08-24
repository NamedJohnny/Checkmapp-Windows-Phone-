using CheckMapp.Model.Tables;
using CheckMapp.ViewModels.TripViewModels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CheckMapp.ViewModels.ArchivesViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class TimelineViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        /// <summary>
        /// Collection de voyage archives
        /// </summary>
        public ObservableCollection<Trip> ArchiveTripList { get; private set; }
        /// <summary>
        /// Initializes a new instance of the TimelineViewModel class.
        /// </summary>
        public TimelineViewModel(INavigationService navigationService)
        {
            Messenger.Default.Register<List<Trip>>(this,
 trips =>
 {
     ArchiveTripList = new ObservableCollection<Trip>(trips);
 });
            this._navigationService = navigationService;
        }

        private ICommand _tripCommand;

        public ICommand TripCommand
        {
            get
            {
                if (_tripCommand == null)
                {
                    _tripCommand = new RelayCommand<int>((id) => TripNav(id));
                }
                return _tripCommand;
            }

        }

        private void TripNav(int id)
        {
            Messenger.Default.Send<int, TripViewModel>(id);
            _navigationService.NavigateTo("TripView");
        }
    }
}