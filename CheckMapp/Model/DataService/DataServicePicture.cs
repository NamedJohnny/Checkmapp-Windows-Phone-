using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System;
using CheckMapp.Model.Tables;

namespace CheckMapp.Model.DataService
{
    public class DataServicePicture : IDataServicePicture
    {
        private DatabaseDataContext db;

        public DataServicePicture()
        {
            db = App.db;
        }

        public void addPicture(Picture newPicture)
        {
            db.pictures.InsertOnSubmit(newPicture);
            db.SubmitChanges();
        }

        public List<Picture> LoadPictures()
        {
            return db.pictures.ToList();
        }

        public List<Picture> LoadPicturesByPoiId(int poiId)
        {
            return db.pictures.Where(x => x.PointOfInterest.Id == poiId).ToList();
        }

        public List<Picture> LoadPicturesFromTrip(Trip trip)
        {
            return db.pictures.Where(x => x.Trip == trip).ToList();
        }

        public Picture getPictureById(int id)
        {
            return db.pictures.Where(x => x.Id == id).First();
        }

        public void DeletePicture(Picture picture)
        {
            var existing = db.pictures.Single(x => x.Id == picture.Id);

            if (existing != null)
            {
                db.pictures.DeleteOnSubmit(existing);
                db.SubmitChanges();
            }
        }

        public void UpdatePicture(Picture picture)
        {
            Picture pictureToUpdate = db.pictures.Where(x => x.Id == picture.Id).First();

            pictureToUpdate.Id = picture.Id;
            pictureToUpdate.PictureData = picture.PictureData;
            pictureToUpdate.PointOfInterest = picture.PointOfInterest;
            pictureToUpdate.Description = picture.Description;

            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to save a picture : " + e);
            }
        }

    }
}
