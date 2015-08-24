using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.ComponentModel;
using CheckMapp.Model.Tables;
using CheckMapp.Model.DataService;
using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Utility = CheckMapp.Utils.Utility;
using FluentValidation;
using CheckMapp.Utils.Validations;
using System.Runtime.Serialization;
using Microsoft.Phone.Shell;
using CheckMapp.Utils.EditableObject;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using CheckMapp.ViewModels.TripViewModels;

namespace CheckMapp.ViewModels.NoteViewModels
{
    [DataContract]
    public class AddEditNoteViewModel
    {
        private Note _note;
        private INavigationService _navigationService;

        // Used for validate the form
        private IValidator<Note> _validator;

        public AddEditNoteViewModel(INavigationService navigationService)
        {
            Messenger.Default.Register<Tuple<int, int, PointOfInterest, Mode>>(this,
 tuple =>
 {
     Init(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
 });
            Messenger.Default.Register<Tuple<int, int, Mode>>(this,
 tuple =>
 {
     Init(tuple.Item1, tuple.Item2, null, tuple.Item3);
 });

            this._navigationService = navigationService;

        }

        private void Init(int trip, int note, PointOfInterest poi, Mode mode)
        {
            this.Mode = mode;

            if (this.Mode == ViewModels.Mode.add)
            {
                Note = new Note();
                DataServiceTrip dsTrip = new DataServiceTrip();
                Note.Trip = dsTrip.getTripById(trip);
                Note.Date = DateTime.Now;
                if (poi != null)
                    this.POISelected = poi;
            }
            else
            {
                Note = GetNoteInDB(note);
            }

            if (Note.Trip.PointsOfInterests != null)
                _poiList = new List<PointOfInterest>(Note.Trip.PointsOfInterests);


            EditableObject = new Caretaker<Note>(this.Note);
            EditableObject.BeginEdit();

            InitialiseValidator();
        }

        private void InitialiseValidator()
        {
            _validator = new ValidatorFactory().GetValidator<Note>();
        }

        /// <summary>
        /// Mon objet editable, nécessaire pour annuler les changements
        /// </summary>
        private Caretaker<Note> EditableObject { get; set; }

        private ICommand _addEditNoteCommand;
        public ICommand AddEditNoteCommand
        {
            get
            {
                if (_addEditNoteCommand == null)
                {
                    _addEditNoteCommand = new RelayCommand(() => AddEditNote());
                }
                return _addEditNoteCommand;
            }

        }

        private ICommand _cancelNoteCommand;
        public ICommand CancelNoteCommand
        {
            get
            {
                if (_cancelNoteCommand == null)
                {
                    _cancelNoteCommand = new RelayCommand(() => CancelNote());
                }
                return _cancelNoteCommand;
            }
        }

        #region Properties


        private bool _isFormValid;

        [DataMember]
        public bool IsFormValid
        {
            get { return _isFormValid; }
            set
            {
                _isFormValid = value;
            }
        }

        /// <summary>
        /// La note
        /// </summary>
        [DataMember]
        public Note Note
        {
            get { return _note; }
            set
            {
                _note = value;
            }
        }

        [DataMember]
        public Mode Mode
        {
            get;
            set;
        }

        private bool _noneCheck;
        /// <summary>
        /// Aucun poi sélectionné
        /// </summary>
        [DataMember]
        public bool NoneCheck
        {
            get { return _noneCheck; }
            set { _noneCheck = value; }
        }

        /// <summary>
        /// Le titre de ma note
        /// </summary>
        [DataMember]
        public string NoteName
        {
            get { return Note.Title; }
            set
            {
                Note.Title = value;
                RaisePropertyChanged("NoteName");
            }
        }

        private List<PointOfInterest> _poiList;
        /// <summary>
        /// Liste des points d'intérêts
        /// </summary>
        [DataMember]
        public List<PointOfInterest> PoiList
        {
            get { return _poiList; }
            set
            {
                _poiList = value;
                RaisePropertyChanged("PoiList");
            }
        }

        /// <summary>
        /// Le point d'intérêt
        /// </summary>
        [DataMember]
        public PointOfInterest POISelected
        {
            get { return Note.PointOfInterest; }
            set
            {
                Note.PointOfInterest = value;
                RaisePropertyChanged("POISelected");
            }
        }

        /// <summary>
        /// Le contenu de ma note
        /// </summary>
        [DataMember]
        public string Message
        {
            get { return Note.Message; }
            set
            {
                Note.Message = value;
                RaisePropertyChanged("Message");
            }
        }

        /// <summary>
        /// La date de ma note
        /// </summary>
        [DataMember]
        public DateTime NoteDate
        {
            get { return Note.Date; }
            set
            {
                Note.Date = value;
                RaisePropertyChanged("NoteDate");
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
            Messenger.Default.Send<int, TripViewModel>(Note.Trip.Id);
            _navigationService.GoBack();
        }

        #endregion

        #region DBMethods

        /// <summary>
        /// Ajouter ou modifier une note
        /// </summary>
        public void AddEditNote()
        {
            // If the form is not valid, a notification will appear
            if (ValidationErrorsHandler.IsValid(_validator, Note))
            {
                _isFormValid = true;

                if (NoneCheck)
                    Note.PointOfInterest = null;

                if (Mode == Mode.add)
                    AddNoteInDB();
                else if (Mode == Mode.edit)
                {
                    UpdateExistingNote();
                    Messenger.Default.Send<int, NoteViewModel>(Note.Id);
                }

                EditableObject.EndEdit();
            }
            else
            {
                _isFormValid = false;
            }
        }

        public void CancelNote()
        {
            if (Mode == ViewModels.Mode.add)
            {
                Note.Trip.Notes.Remove(Note);
                Note.Trip = null;
            }

            EditableObject.CancelEdit();
        }

        private void AddNoteInDB()
        {
            // Workaround to retrieve the REAL associated trip. 
            // When the app is tombstoned, it creates a copy of the current trip.
            // So when comes the time to save the note in the database, it saves a new trip
            // and duplicate all existing notes attached to this new trip !         
            // The result is an Absolute Mess...

            // Calling the current trip method make sure that we deal with the correct data
            // and not simply a copy.
            Note.Trip.Notes.Add(Note);

            DataServiceNote dsNote = new DataServiceNote();
            dsNote.addNote(Note);
        }

        private void UpdateExistingNote()
        {
            DataServiceNote dsNote = new DataServiceNote();
            dsNote.UpdateNote(Note);
        }

        private Note GetNoteInDB(int noteId)
        {
            DataServiceNote dsNote = new DataServiceNote();
            return dsNote.getNoteById(noteId);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}