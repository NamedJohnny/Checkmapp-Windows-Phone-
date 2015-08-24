using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System;
using CheckMapp.Model.Tables;

namespace CheckMapp.Model.DataService
{
    public class DataServiceNote : IDataServiceNote
    {
        private DatabaseDataContext db;

        public DataServiceNote()
        {
            db = App.db;
        }

        public void addNote(Note newNote)
        {
            db.notes.InsertOnSubmit(newNote);
            db.SubmitChanges();
        }

        public Note getNoteById(int id)
        {
            return db.notes.Where(x => x.Id == id).First();
        }

        public List<Note> LoadNotes()
        {
            return db.notes.ToList();
        }

        public List<Note> LoadNotesFromTrip(Trip trip)
        {
            return db.notes.Where(x => x.Trip == trip).ToList();
        }

        public List<Note> LoadNotesByPoiId(int poiId)
        {
            return db.notes.Where(x => x.PointOfInterest.Id == poiId).ToList();
        }

        public void UpdateNote(Note note)
        {
            Note noteToUpdate = db.notes.Where(x => x.Id == note.Id).First();

            noteToUpdate.Message = note.Message;
            noteToUpdate.PointOfInterest = note.PointOfInterest;
            noteToUpdate.Title = note.Title;

            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to save a note : " + e);
            }
        }

        public void DeleteNote(Note note)
        {
            var existing = db.notes.Single(x => x.Id == note.Id);

            if (existing != null)
            {
                db.notes.DeleteOnSubmit(existing);
                db.SubmitChanges();
            }
        }
    }
}