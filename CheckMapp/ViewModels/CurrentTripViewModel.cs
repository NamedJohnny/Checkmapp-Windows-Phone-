using CheckMapp.Model;
using GalaSoft.MvvmLight;
using System;

namespace CheckMapp.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class CurrentTripViewModel : ViewModelBase
    {
        private Trip _currentTrip;

        /// <summary>
        /// Initializes a new instance of the TripViewModel class.
        /// </summary>
        public CurrentTripViewModel()
        {
            Trip trip = new Trip();
            trip.Name = "Australie en famille 2014";
            trip.BeginDate = DateTime.Now.AddDays(-20);
            trip.EndDate = DateTime.Now.AddDays(-12);
            CurrentTrip = trip;
        }

        public Trip CurrentTrip
        {
            get { return _currentTrip; }
            set { _currentTrip = value; }
        }

    }
}