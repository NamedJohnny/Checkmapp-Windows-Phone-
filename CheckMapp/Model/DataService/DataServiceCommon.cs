using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckMapp.Model.Tables;

namespace CheckMapp.Model.DataService
{
    public class DataServiceCommon : IDataServiceCommon
    {
        private DataServiceNote dsNote;
        private DataServicePicture dsPicture;
        private DataServicePoi dsPoi;
        private DataServiceTrip dsTrip;

        public DataServiceCommon()
        {
            dsNote = new DataServiceNote();
            dsPicture = new DataServicePicture();
            dsPoi = new DataServicePoi();
            dsTrip = new DataServiceTrip();
        }

        public void DeletePoiInCascade(PointOfInterest poi)
        {
            List<Note> noteList = dsNote.LoadNotesByPoiId(poi.Id);
            List<Picture> pictureList = dsPicture.LoadPicturesByPoiId(poi.Id);

            // Removing items
            DeleteNotesAndPictures(noteList, pictureList);
        }

        public void DeleteTripInCascade(Trip trip)
        {
            List<Note> noteList = dsNote.LoadNotesFromTrip(trip);
            List<Picture> pictureList = dsPicture.LoadPicturesFromTrip(trip);
            List<PointOfInterest> poiList = dsPoi.LoadPointOfInterestsFromTrip(trip).ToList();

            // Removing items
            DeleteNotesAndPictures(noteList, pictureList);
            DeletePois(poiList, true);
        }

        private void DeleteNotesAndPictures(List<Note> noteList, List<Picture> pictureList)
        {
            // Delete all notes associated
            foreach (Note note in noteList)
            {
                dsNote.DeleteNote(note);
            }

            // Delete all pictures associated
            foreach (Picture picture in pictureList)
            {
                dsPicture.DeletePicture(picture);
            }
        }

        private void DeletePois(List<PointOfInterest> poiList, bool deleteCascade)
        {
            // Delete all POIs associated
            foreach (PointOfInterest poi in poiList)
            {
                dsPoi.DeletePoi(poi, deleteCascade);
            }
        }
    }
}
