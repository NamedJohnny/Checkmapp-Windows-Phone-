using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using CheckMapp.Model.Tables;

namespace CheckMapp.Model
{
    public class DatabaseDataContext : DataContext
    {
        public DatabaseDataContext(string connectionString)
            : base(connectionString)
        { }

        public Table<Trip> trips;
        public Table<Note> notes;
        public Table<PointOfInterest> pointsOfInterests;
        public Table<Picture> pictures;
    }
}
