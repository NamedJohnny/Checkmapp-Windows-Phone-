using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System;
using CheckMapp.Model.Tables;
using CheckMapp.Model.DataService;
using CheckMapp.KeyGroup;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using CheckMapp.ViewModels.TripViewModels;

namespace CheckMapp.ViewModels.NoteViewModels
{
    public class ListNoteViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        public ListNoteViewModel(INavigationService navigationService)
        {
            Messenger.Default.Register<Tuple<int, PointOfInterest>>(this,
tuple =>
{
    DataServiceTrip dsTrip = new DataServiceTrip();
    this.Trip = dsTrip.getTripById(tuple.Item1);
    this.PoiLoaded = tuple.Item2;
});

            Messenger.Default.Register<int>(this,
trip =>
{
    DataServiceTrip dsTrip = new DataServiceTrip();
    this.Trip = dsTrip.getTripById(trip);
    this.PoiLoaded = null;
});
            this._navigationService = navigationService;
        }

        public List<Note> NoteList()
        {
            if(this.PoiLoaded!=null)
                return this.Trip.Notes.Where(x => (x.PointOfInterest != null) && (x.PointOfInterest == PoiLoaded)).ToList();
            return Trip.Notes.ToList();
        }

        /// <summary>
        /// Le voyage choisi
        /// </summary>
        public Trip Trip
        {
            get;
            set;
        }

        public List<KeyedList<string, Note>> GroupedNotes
        {
            get
            {
                var groupedNotes =
                    from note in NoteList()
                    orderby note.Date
                    group note by note.Date.ToString("m") into notesByDay
                    select new KeyedList<string, Note>(notesByDay);

                return new List<KeyedList<string, Note>>(groupedNotes);
            }
        }

        private ICommand _noteCommand;

        public ICommand NoteCommand
        {
            get
            {
                if (_noteCommand == null)
                {
                    _noteCommand = new RelayCommand<int>((id) => NoteNav(id));
                }
                return _noteCommand;
            }
        }

        private void NoteNav(int id)
        {
            Messenger.Default.Send<Tuple<int, List<Note>>, NoteViewModel>(new Tuple<int, List<Note>>(id, NoteList()));
            _navigationService.NavigateTo("NoteView");
        }

        private ICommand _tripCommand;

        public ICommand TripCommand
        {
            get
            {
                if (_tripCommand == null)
                {
                    _tripCommand = new RelayCommand(() => Messenger.Default.Send<int, TripViewModel>(Trip.Id));
                }
                return _tripCommand;
            }
        }


        private ICommand _editNoteCommand;

        public ICommand EditNoteCommand
        {
            get
            {
                if (_editNoteCommand == null)
                {
                    _editNoteCommand = new RelayCommand<Tuple<int, Mode>>((tuple) => EditNoteNav(tuple));
                }
                return _editNoteCommand;
            }
        }

        private void EditNoteNav(Tuple<int, Mode> tuple)
        {
            Messenger.Default.Send<Tuple<int, int, PointOfInterest, Mode>, AddEditNoteViewModel>(new Tuple<int, int, PointOfInterest, Mode>(Trip.Id, tuple.Item1, PoiLoaded, tuple.Item2));
            _navigationService.NavigateTo("AddEditNoteView");
        }

        private ICommand _deleteNoteCommand;
        public ICommand DeleteNoteCommand
        {
            get
            {
                if (_deleteNoteCommand == null)
                {
                    _deleteNoteCommand = new RelayCommand<Note>((note) => DeleteNote(note));
                }
                return _deleteNoteCommand;
            }

        }

        private ICommand _deleteNotesCommand;
        public ICommand DeleteNotesCommand
        {
            get
            {
                if (_deleteNotesCommand == null)
                {
                    _deleteNotesCommand = new RelayCommand<List<object>>((noteList) => DeleteNotes(noteList));
                }
                return _deleteNotesCommand;
            }

        }

        /// <summary>
        /// Si les notes sont selon un poi
        /// </summary>
        public PointOfInterest PoiLoaded
        {
            get;
            set;
        }


        #region DBMethods

        public void DeleteNotes(List<object> noteList)
        {
            DataServiceNote dsNote = new DataServiceNote();
            foreach (Note note in noteList)
            {
                Trip.Notes.Remove(note);
                dsNote.DeleteNote(note);
            }

            TripCommand.Execute(null);
        }

        public void DeleteNote(Note noteSelected)
        {
            DataServiceNote dsNote = new DataServiceNote();
            Trip.Notes.Remove(noteSelected);
            dsNote.DeleteNote(noteSelected);

            TripCommand.Execute(null);
        }

        #endregion
    }
}