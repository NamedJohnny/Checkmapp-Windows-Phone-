using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System;
using CheckMapp.Model.Tables;
using CheckMapp.Resources;

namespace CheckMapp.Model.DataService
{
    public class DataServicePoi : IDataServicePoi
    {
        private DatabaseDataContext db;

        public DataServicePoi()
        {
            db = App.db;
        }

        public void addPoi(PointOfInterest poi)
        {
            db.pointsOfInterests.InsertOnSubmit(poi);
            db.SubmitChanges();
        }

        public PointOfInterest getPOIById(int id)
        {
            return db.pointsOfInterests.Where(x => x.Id == id).First();
        }

        public ObservableCollection<PointOfInterest> LoadPointOfInterestsFromTrip(Trip trip)
        {
            List<PointOfInterest> listPOI = db.pointsOfInterests.Where(x => x.Trip == trip).ToList();
            return new ObservableCollection<PointOfInterest>(listPOI);
        }

        public void DeletePoi(PointOfInterest poi, bool deleteCascade)
        {
            var existing = db.pointsOfInterests.FirstOrDefault(x => x.Id == poi.Id);

            if (existing != null)
            {
                if (deleteCascade)
                {
                    DataServiceCommon dsCommon = new DataServiceCommon();
                    dsCommon.DeletePoiInCascade(poi);
                }

                db.pointsOfInterests.DeleteOnSubmit(existing);
                db.SubmitChanges();
            }
        }

        public void UpdatePoi(PointOfInterest poi)
        {
            PointOfInterest poiToUpdate = db.pointsOfInterests.First(x => x.Id == poi.Id);

            poiToUpdate.Location = poi.Location;
            poiToUpdate.Name = poi.Name;
            poiToUpdate.Latitude = poi.Latitude;
            poiToUpdate.Longitude = poi.Longitude;

            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to save a point of interest : " + e);
            }
        }
    }
}
