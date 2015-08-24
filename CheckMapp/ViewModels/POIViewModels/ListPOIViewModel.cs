using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CheckMapp.Model.DataService;
using CheckMapp.Model.Tables;
using System.Collections.Generic;
using System.Device.Location;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using CheckMapp.ViewModels.PhotoViewModels;
using CheckMapp.ViewModels.NoteViewModels;
using CheckMapp.ViewModels.TripViewModels;

namespace CheckMapp.ViewModels.POIViewModels
{
    public class ListPOIViewModel : ViewModelBase
    {

        private INavigationService _navigationService;
        /// <summary>
        /// Initializes a new instance of the POIViewModel class.
        /// </summary>
        public ListPOIViewModel(INavigationService navigationService)
        {
            Messenger.Default.Register<int>(this,
trip =>
{
    DataServiceTrip dsTrip = new DataServiceTrip();
    this.Trip = dsTrip.getTripById(trip);
    PointOfInterestList = new ObservableCollection<PointOfInterest>(Trip.PointsOfInterests);
});

            this._navigationService = navigationService;
        }

        #region Properties

        /// <summary>
        /// Le voyage courant
        /// </summary>
        public Trip Trip
        {
            get;
            set;
        }

        private ObservableCollection<PointOfInterest> _pointOfInterestList;
        /// <summary>
        /// La liste des points d'intérêts
        /// </summary>
        public ObservableCollection<PointOfInterest> PointOfInterestList
        {
            get { return _pointOfInterestList; }
            set
            {
                _pointOfInterestList = value;
                RaisePropertyChanged("PointOfInterestList");
            }
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

        private ICommand _addPOINearCommand;

        public ICommand AddPOINearCommand
        {
            get
            {
                if (_addPOINearCommand == null)
                {
                    _addPOINearCommand = new RelayCommand<PointOfInterest>((poi) => AddPOINearNav(poi));
                }
                return _addPOINearCommand;
            }
        }

        private void AddPOINearNav(PointOfInterest poi)
        {
            Messenger.Default.Send<Tuple<PointOfInterest, Mode>, AddEditPOIViewModel>(new Tuple<PointOfInterest, Mode>(poi, Mode.addFromExisting));
            _navigationService.NavigateTo("AddEditPOIView");
        }

        private bool _loading = false;

        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged("Loading");
            }
        }

        private ICommand _deletePOIsCommand;
        public ICommand DeletePOIsCommand
        {
            get
            {
                if (_deletePOIsCommand == null)
                {
                    _deletePOIsCommand = new RelayCommand<List<object>>((poiList) => DeletePOIs(poiList));
                }
                return _deletePOIsCommand;
            }

        }

        private ICommand _deletePOICommand;
        public ICommand DeletePOICommand
        {
            get
            {
                if (_deletePOICommand == null)
                {
                    _deletePOICommand = new RelayCommand<PointOfInterest>((poi) => DeletePOI(poi));
                }
                return _deletePOICommand;
            }
        }

        /// <summary>
        /// Savoir si on supprime les objets reliés au POI ou on les met a null
        /// </summary>
        public bool DeletePOIObject
        {
            get;
            set;
        }


        private ICommand _editPOICommand;

        public ICommand EditPOICommand
        {
            get
            {
                if (_editPOICommand == null)
                {
                    _editPOICommand = new RelayCommand<Tuple<int, Mode>>((tuple) => EditPOINav(tuple));
                }
                return _editPOICommand;
            }
        }

        private void EditPOINav(Tuple<int, Mode> tuple)
        {
            Messenger.Default.Send<Tuple<int, int, Mode>, AddEditPOIViewModel>(new Tuple<int, int, Mode>(Trip.Id, tuple.Item1, tuple.Item2));
            _navigationService.NavigateTo("AddEditPOIView");
        }

        private ICommand _listPhotoCommand;

        public ICommand ListPhotoCommand
        {
            get
            {
                if (_listPhotoCommand == null)
                {
                    _listPhotoCommand = new RelayCommand<PointOfInterest>((poi) => ListPhotoNav(poi));
                }
                return _listPhotoCommand;
            }
        }

        private void ListPhotoNav(PointOfInterest poi)
        {
            Messenger.Default.Send<Tuple<int, PointOfInterest>, ListPhotoViewModel>(new Tuple<int, PointOfInterest>(Trip.Id, poi));
            _navigationService.NavigateTo("ListPhotoView");
        }

        private ICommand _listNoteCommand;

        public ICommand ListNoteCommand
        {
            get
            {
                if (_listNoteCommand == null)
                {
                    _listNoteCommand = new RelayCommand<PointOfInterest>((poi) => ListNoteNav(poi));
                }
                return _listNoteCommand;
            }
        }

        private void ListNoteNav(PointOfInterest poi)
        {
            Messenger.Default.Send<Tuple<int, PointOfInterest>, ListNoteViewModel>(new Tuple<int, PointOfInterest>(Trip.Id, poi));
            _navigationService.NavigateTo("ListNoteView");
        }

        #endregion


        #region DBMethods

        public void DeletePOI(PointOfInterest poi)
        {
            if (DeletePOIObject)
            {
                // For some reasons, Picture table doesn't refresh properly
                // We have to remove each element in the array manually
                DataServicePicture dsPicture = new DataServicePicture();
                foreach (Picture pic in dsPicture.LoadPicturesByPoiId(poi.Id))
                {
                    Trip.Pictures.Remove(pic);
                    dsPicture.DeletePicture(pic);
                }

                DataServiceNote dsNotes = new DataServiceNote();
                foreach (Note note in dsNotes.LoadNotesByPoiId(poi.Id))
                {
                    Trip.Notes.Remove(note);
                    dsNotes.DeleteNote(note);
                }
            }
            else
            {
                // For some reasons, Picture table doesn't refresh properly
                // We have to remove each element in the array manually
                DataServicePicture dsPicture = new DataServicePicture();
                foreach (Picture pic in dsPicture.LoadPicturesByPoiId(poi.Id))
                    pic.PointOfInterest = null;

                DataServiceNote dsNotes = new DataServiceNote();
                foreach (Note note in dsNotes.LoadNotesByPoiId(poi.Id))
                    note.PointOfInterest = null;
            }

            DataServicePoi dsPoi = new DataServicePoi();
            Trip.PointsOfInterests.Remove(poi);
            PointOfInterestList.Remove(poi);

            dsPoi.DeletePoi(poi, false);

            TripCommand.Execute(null);
        }

        public void DeletePOIs(List<object> poiList)
        {
            foreach (PointOfInterest poi in poiList)
            {
                DeletePOI(poi);
            }

            TripCommand.Execute(null);
        }


        #endregion
    }
}