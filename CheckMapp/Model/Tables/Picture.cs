using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace CheckMapp.Model.Tables
{
    [Table(Name = "Picture")]
    [DataContract(IsReference = true)]
    public class Picture : INotifyPropertyChanged, INotifyPropertyChanging
    {
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

        [Column]
        private int? _pointOfInterestId;
        private EntityRef<PointOfInterest> _pointOfInterest = new EntityRef<PointOfInterest>();
        [Association(Storage = "_pointOfInterest", ThisKey = "_pointOfInterestId", OtherKey = "Id", IsForeignKey = true)]
        [DataMember]
        public PointOfInterest PointOfInterest
        {
            get { return _pointOfInterest.Entity; }
            set
            {
                NotifyPropertyChanging("PointOfInterest");
                _pointOfInterest.Entity = value;

                if (value != null)
                {
                    _pointOfInterestId = value.Id;
                }

                NotifyPropertyChanging("PointOfInterest");
            }
        }

        private DateTime _date = new DateTime();

        [Column]
        [DataMember]
        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (!_date.Equals(value))
                {
                    NotifyPropertyChanging("Date");
                    _date = value;
                    NotifyPropertyChanged("Date");
                }
            }
        }

        private string _description;

        [Column]
        [DataMember]
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    NotifyPropertyChanging("Description");
                    _description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        private byte[] _pictureData;

        [Column (DbType = "image", UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public byte[] PictureData
        {
            get { return _pictureData; }
            set
            {
                if (_pictureData != value)
                {
                    NotifyPropertyChanging("PictureData");
                    _pictureData = value;
                    NotifyPropertyChanged("PictureData");
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
