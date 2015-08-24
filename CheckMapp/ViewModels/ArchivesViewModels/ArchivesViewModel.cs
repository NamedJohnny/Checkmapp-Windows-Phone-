using CheckMapp.Model.DataService;
using CheckMapp.Model.Tables;
using CheckMapp.ViewModel;
using CheckMapp.ViewModels.TripViewModels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CheckMapp.ViewModels.ArchivesViewModels
{
    public class ArchivesViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        /// <summary>
        /// Collection de voyage archives
        /// </summary>
        public ObservableCollection<Trip> ArchiveTripList { get; private set; }

        public ArchivesViewModel(INavigationService navigationService)
        {
            Messenger.Default.Register<List<Trip>>(this,
  trips =>
  {
      ArchiveTripList = new ObservableCollection<Trip>(trips);
  });

            
            this._navigationService = navigationService;
        }

        private ICommand _deleteTripCommand;
        public ICommand DeleteTripCommand
        {
            get
            {
                if (_deleteTripCommand == null)
                {
                    _deleteTripCommand = new RelayCommand<Trip>((trip) => DeleteTrip(trip));
                }
                return _deleteTripCommand;
            }

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

        private void DeleteTrip(Trip trip)
        {
            DataServiceTrip dsTrip = new DataServiceTrip();
            dsTrip.DeleteTrip(trip);
            Messenger.Default.Send<List<Trip>, TimelineViewModel>(ArchiveTripList.ToList());
        }

        

    }
}
