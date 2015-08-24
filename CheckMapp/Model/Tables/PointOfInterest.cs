using CheckMapp.ViewModels;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Device.Location;
using System.Runtime.Serialization;

namespace CheckMapp.Model.Tables
{
    [Table(Name = "PointOfInterest")]
    [DataContract(IsReference = true)]
    public class PointOfInterest : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Constructors

        public PointOfInterest()
        {
            /* _notes = new EntitySet<Note>();
             _pictures = new EntitySet<Picture>();*/
        }

        #endregion

        #region Members

        private int _id;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        [DataMember]
        public int Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    NotifyPropertyChanging("Id");
                    _id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        private string _name;

        [Column]
        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    NotifyPropertyChanging("Name");
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        private string _location;

        [Column]
        [DataMember]
        public string Location
        {
            get { return _location; }
            set
            {
                if (_location != value)
                {
                    NotifyPropertyChanging("Location");
                    _location = value;
                    NotifyPropertyChanged("Location");
                }
            }
        }

        private double _longitude;

        [Column]
        [DataMember]
        public double Longitude
        {
            get { return _longitude; }
            set
            {
                if (_longitude != value)
                {
                    NotifyPropertyChanging("Longitude");
                    _longitude = value;
                    NotifyPropertyChanged("Longitude");
                }
            }
        }

        private double _latitude;

        [Column]
        [DataMember]
        public double Latitude
        {
            get { return _latitude; }
            set
            {
                if (_latitude != value)
                {
                    NotifyPropertyChanging("Latitude");
                    _latitude = value;
                    NotifyPropertyChanged("Latitude");
                }
            }
        }

        public GeoCoordinate Coordinate
        {
            get
            {
                return new GeoCoordinate(Latitude, Longitude);
            }
        }

        private POIType _type;
        [Column]
        public POIType Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    NotifyPropertyChanging("Type");
                    _type = value;
                    NotifyPropertyChanged("Type");
                }
            }
        }

        [Column]
        private int? _tripId;
        private EntityRef<Trip> _trip = new EntityRef<Trip>();
        [Association(Storage = "_trip", ThisKey = "_tripId", OtherKey = "Id", IsForeignKey = true)]
        [DataMember]
        public Trip Trip
        {
            get { return _trip.Entity; }
            set
            {
                NotifyPropertyChanging("Trip");
                _trip.Entity = value;

                if (value != null)
                {
                    _tripId = value.Id;
                }

                NotifyPropertyChanging("Trip");
            }
        }

        // Version column improves update performance.
        #pragma warning disable 169
        [Column(IsVersion = true)]
        private Binary _version;

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify that a property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}
