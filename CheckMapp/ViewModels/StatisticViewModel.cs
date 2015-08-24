using CheckMapp.Model.DataService;
using CheckMapp.Model.Tables;
using CheckMapp.Resources;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CheckMapp.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class StatisticViewModel : ViewModelBase
    {
        private Trip _trip;

        /// <summary>
        /// Initializes a new instance of the StatisticViewModel class.
        /// </summary>
        public StatisticViewModel()
        {
            Messenger.Default.Register<int>(this,
trip =>
{
    DataServiceTrip dsTrip = new DataServiceTrip();
    this.Trip = dsTrip.getTripById(trip);
    PointOfInterestList = new ObservableCollection<PointOfInterest>(Trip.PointsOfInterests);
    TripFriends = new ObservableCollection<string>(Utils.Utility.FriendToList(Trip.FriendList));
});


        }

        #region Properties

        public Trip Trip
        {
            get { return _trip; }
            set
            {
                _trip = value;
            }
        }

        private ObservableCollection<string> _tripFriends;
        public ObservableCollection<string> TripFriends
        {
            get { return _tripFriends; }
            set
            {
                _tripFriends = value;
                RaisePropertyChanged("TripFriends");
            }
        }

        private ObservableCollection<PointOfInterest> _pointOfInterestList;
        public ObservableCollection<PointOfInterest> PointOfInterestList
        {
            get { return _pointOfInterestList; }
            set
            {
                _pointOfInterestList = value;
                RaisePropertyChanged("PointOfInterestList");
            }
        }

        public bool HasFriends
        {
            get
            {
                return !String.IsNullOrEmpty(Trip.FriendList);
            }
        }
        public bool IsActif
        {
            get
            {
                return Trip.IsActif;
            }
        }

        public string TripName
        {
            get { return Trip.Name; }
        }

        public string TripBeginDate
        {
            get { return String.Format(AppResources.TripBeginDate, Trip.BeginDate.ToShortDateString()); }
        }

        public string TripEndDate
        {
            get
            {
                if (Trip.EndDate != null)
                {
                    return String.Format(AppResources.TripEndDate, Trip.EndDate.Value.ToShortDateString());
                }
                else
                    return null;
            }
        }

        public string TripNoteToday
        {
            get { return String.Format(AppResources.NoteToday, Trip.Notes.Where(x => x.Date.Date == DateTime.Today).Count()); }
        }

        public string TripNoteAllTime
        {
            get { return String.Format(AppResources.NoteAllTime, Trip.Notes.Count()); }
        }

        public string TripPictureToday
        {
            get { return String.Format(AppResources.PictureToday, Trip.Pictures.Where(x => x.Date.Date == DateTime.Today).Count()); }
        }

        public string TripPictureAllTime
        {
            get { return String.Format(AppResources.PictureAllTime, Trip.Pictures.Count()); }
        }

        #endregion

    }
}