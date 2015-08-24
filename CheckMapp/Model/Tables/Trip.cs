using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using CheckMapp.Model.Utils;
using System.Runtime.Serialization;
using System.Device.Location;

namespace CheckMapp.Model.Tables
{

    [Table(Name = "Trip")]
    [DataContract(IsReference = true)] 
    public class Trip : INotifyPropertyChanged, INotifyPropertyChanging
    {

        #region Constructors
        
        public Trip()
        {
            _notes = new EntitySet<Note>();
            _pictures = new EntitySet<Picture>();
            _pointsOfInterests = new EntitySet<PointOfInterest>();
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

        private string _friendList;

        //exemple: john,felix,roy
        [Column]
        [DataMember]
        public string FriendList
        {
            get { return _friendList; }
            set
            {
                if (_friendList != value)
                {
                    NotifyPropertyChanging("FriendList");
                    _friendList = value;
                    NotifyPropertyChanged("FriendList");
                }
            }
        }

        private DateTime _beginDate;

        [Column]
        [DataMember]
        public DateTime BeginDate
        {
            get { return _beginDate; }
            set
            {
                if (!_beginDate.Equals(value))
                {
                    NotifyPropertyChanging("BeginDate");
                    _beginDate = value;
                    NotifyPropertyChanged("BeginDate");
                }
            }
        }

        private DateTime? _endDate;

        [Column]
        [DataMember]
        public DateTime? EndDate
        {
            get { return _endDate; }
            set
            {
                if (!_endDate.Equals(value))
                {
                    NotifyPropertyChanging("EndDate");
                    _endDate = value;
                    NotifyPropertyChanged("EndDate");
                }
            }
        }

        private double _departureLongitude;
      
        [Column]
        [DataMember]
        public double DepartureLongitude
        {
            get { return _departureLongitude; }
            set
            {
                if (_departureLongitude != value)
                {
                    NotifyPropertyChanging("DepartureLongitude");
                    _departureLongitude = value;
                    NotifyPropertyChanged("DepartureLongitude");
                }
            }
        }

        private double _departureLatitude;
       
        [Column]
        [DataMember]
        public double DepartureLatitude
        {
            get { return _departureLatitude; }
            set
            {
                if (_departureLatitude != value)
                {
                    NotifyPropertyChanging("DepartureLatitude");
                    _departureLatitude = value;
                    NotifyPropertyChanged("DepartureLatitude");
                }
            }
        }

        private double _destinationLongitude;

        [Column]
        [DataMember]
        public double DestinationLongitude
        {
            get { return _destinationLongitude; }
            set
            {
                if (_destinationLongitude != value)
                {
                    NotifyPropertyChanging("DestinationLongitude");
                    _destinationLongitude = value;
                    NotifyPropertyChanged("DestinationLongitude");
                }
            }
        }

        private double _destinationLatitude;

        [Column]
        [DataMember]
        public double DestinationLatitude
        {
            get { return _destinationLatitude; }
            set
            {
                if (_destinationLatitude != value)
                {
                    NotifyPropertyChanging("DestinationLatitude");
                    _destinationLatitude = value;
                    NotifyPropertyChanged("DestinationLatitude");
                }
            }
        }

        private byte[] _mainPictureData;

        [Column(DbType = "image", UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public byte[] MainPictureData
        {
            get { return _mainPictureData; }
            set
            {
                if (_mainPictureData != value)
                {
                    NotifyPropertyChanging("MainPictureData");
                    _mainPictureData = value;
                    NotifyPropertyChanged("MainPictureData");
                }
            }
        }

        private EntitySet<Note> _notes = new EntitySet<Note>();

        [Association(Storage = "_notes", OtherKey = "_tripId", ThisKey = "Id")]
        [DataMember]
        public EntitySet<Note> Notes
        {
            get {
                if (_notes == null)
                    _notes = new EntitySet<Note>();
                return _notes; 
            }
            set
            {
                if (_notes == null)
                {
                    _notes = new EntitySet<Note>();
                }

                if (_notes != value)
                {
                    NotifyPropertyChanging("Notes");
                    _notes.Assign(value);
                    NotifyPropertyChanged("Notes");
                }
            }
        }

        private EntitySet<Picture> _pictures = new EntitySet<Picture>();

        [Association(Storage = "_pictures", OtherKey = "_tripId", ThisKey = "Id")]
        [DataMember]
        public EntitySet<Picture> Pictures
        {
            get {
                if (_pictures == null)
                    _pictures = new EntitySet<Picture>();
                return _pictures; 
            }
            set
            {
                if (_pictures == null)
                {
                    _pictures = new EntitySet<Picture>();
                }

                if (_pictures != value)
                {
                    NotifyPropertyChanging("Pictures");
                    _pictures.Assign(value);
                    NotifyPropertyChanged("Pictures");
                }
            }
        }

        private EntitySet<PointOfInterest> _pointsOfInterests = new EntitySet<PointOfInterest>();

        [Association(Storage = "_pointsOfInterests", OtherKey = "_tripId", ThisKey = "Id")]
        [DataMember]
        public EntitySet<PointOfInterest> PointsOfInterests
        {
            get {
                if (_pointsOfInterests == null)
                    _pointsOfInterests = new EntitySet<PointOfInterest>();
                return _pointsOfInterests; 
            }
            set
            {
                if (_pointsOfInterests == null)
                {
                    _pointsOfInterests = new EntitySet<PointOfInterest>();
                }

                if (_pointsOfInterests != value)
                {
                    NotifyPropertyChanging("PointsOfInterests");
                    _pointsOfInterests.Assign(value);
                    NotifyPropertyChanged("PointsOfInterests");
                }
            }
        }

        // Version column improves update performance.
        #pragma warning disable 169
        [Column(IsVersion = true)]
        private Binary _version;

        public bool IsActif
        {
            get { return (EndDate == null); }
        }

        public GeoCoordinate CoordinateDestination
        {
            get
            {
                return new GeoCoordinate(DestinationLatitude, DestinationLongitude);
            }
        }

        public GeoCoordinate CoordinateDeparture
        {
            get
            {
                return new GeoCoordinate(DepartureLatitude, DepartureLongitude);
            }
        }

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
