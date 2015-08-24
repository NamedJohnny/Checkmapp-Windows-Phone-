using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using CheckMapp.Model;
using System;
using System.Linq;
using System.ComponentModel;
using System.Windows.Input;
using CheckMapp.Model.Tables;
using CheckMapp.Model.DataService;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using CheckMapp.ViewModels.TripViewModels;
using System.Collections.Generic;

namespace CheckMapp.ViewModels.NoteViewModels
{
    public class NoteViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        private Note _note;

        public NoteViewModel(INavigationService navigationService)
        {
            Messenger.Default.Register<Tuple<int, List<Note>>>(this,
 tuple =>
 {
     DataServiceNote dsNote = new DataServiceNote();
     this.Note = dsNote.getNoteById(tuple.Item1);
     NoteList = tuple.Item2;
     TripId = Note.Trip.Id;
 });

            Messenger.Default.Register<int>(this,
note =>
{
    DataServiceNote dsNote = new DataServiceNote();
    this.Note = dsNote.getNoteById(note);
    TripId = Note.Trip.Id;
});
            this._navigationService = navigationService;
            
        }

        #region Properties

        public int TripId
        {
            get;
            set;
        }

        public List<Note> NoteList
        {
            get;
            set;
        }

        /// <summary>
        /// La Note actuelle
        /// </summary>
        public Note Note
        {
            get { return _note; }
            set
            {
                _note = value;
                RaisePropertyChanged("Note");
                RaisePropertyChanged("IsVisible");
            }
        }

        public int SelectedIndex
        {
            get
            {
                return NoteList.IndexOf(Note);
            }
        }

        public void NextNote()
        {
            Note = NoteList[SelectedIndex + 1];
        }

        public void PreviousNote()
        {
            Note = NoteList[SelectedIndex - 1];
        }

        /// <summary>
        /// Si le point d'intérêt s'affiche
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return (Note.PointOfInterest != null);
            }
        }

        private ICommand _goBackCommand;

        public ICommand GoBackCommand
        {
            get
            {
                if (_goBackCommand == null)
                {
                    _goBackCommand = new RelayCommand(() => GoBackNav());
                }
                return _goBackCommand;
            }
        }

        private void GoBackNav()
        {
            Messenger.Default.Send<int, TripViewModel>(TripId);
            _navigationService.GoBack();
        }

        private ICommand _editNoteCommand;

        public ICommand EditNoteCommand
        {
            get
            {
                if (_editNoteCommand == null)
                {
                    _editNoteCommand = new RelayCommand<Mode>((mode) => EditNoteNav(mode));
                }
                return _editNoteCommand;
            }
        }

        private void EditNoteNav(Mode mode)
        {
            Messenger.Default.Send<Tuple<int, int, Mode>, AddEditNoteViewModel>(new Tuple<int, int, Mode>(Note.Trip.Id, Note.Id, mode));
            _navigationService.NavigateTo("AddEditNoteView");
        }

        #endregion

        #region Buttons

        private ICommand _deleteNoteCommand;
        public ICommand DeleteNoteCommand
        {
            get
            {
                if (_deleteNoteCommand == null)
                {
                    _deleteNoteCommand = new RelayCommand(() => DeleteNote());
                }
                return _deleteNoteCommand;
            }

        }

        #endregion

        #region DBMethods

        /// <summary>
        /// Supprimer une note
        /// </summary>
        public void DeleteNote()
        {
            DataServiceNote dsNote = new DataServiceNote();
            dsNote.DeleteNote(Note);
            GoBackCommand.Execute(null);
        }

        #endregion
    }
}