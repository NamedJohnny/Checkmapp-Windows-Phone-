using CheckMapp.KeyGroup;
using CheckMapp.Model.Tables;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using System.Linq;
using CheckMapp.Model.DataService;
using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using CheckMapp.ViewModels.TripViewModels;

namespace CheckMapp.ViewModels.PhotoViewModels
{
    public class ListPhotoViewModel : ViewModelBase
    {
        private INavigationService _navigationService;

        public ListPhotoViewModel(INavigationService navigationService)
        {
            Messenger.Default.Register<Tuple<int, PointOfInterest>>(this,
tuple =>
{
    DataServiceTrip dsTrip = new DataServiceTrip();
    this.Trip = dsTrip.getTripById(tuple.Item1);
    this.PoiLoaded = tuple.Item2;
    Loading = true;
});

            Messenger.Default.Register<int>(this,
trip =>
{
    DataServiceTrip dsTrip = new DataServiceTrip();
    this.Trip = dsTrip.getTripById(trip);
    Loading = true;
    this.PoiLoaded = null;
});

            this._navigationService = navigationService;
        }

        #region Properties

        /// <summary>
        /// Le voyage choisi
        /// </summary>
        public Trip Trip
        {
            get;
            set;
        }

        /// <summary>
        /// Si c'est des photos à partir d'un poi
        /// </summary>
        public PointOfInterest PoiLoaded
        {
            get;
            set;
        }


        private ICommand _deletePictureCommand;
        public ICommand DeletePictureCommand
        {
            get
            {
                if (_deletePictureCommand == null)
                {
                    _deletePictureCommand = new RelayCommand<Picture>((picture) => DeletePicture(picture));
                }
                return _deletePictureCommand;
            }

        }

        private ICommand _deletePicturesCommand;
        public ICommand DeletePicturesCommand
        {
            get
            {
                if (_deletePicturesCommand == null)
                {
                    _deletePicturesCommand = new RelayCommand<List<object>>((pictureList) => DeletePictures(pictureList));
                }
                return _deletePicturesCommand;
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


        private ICommand _photoCommand;

        public ICommand PhotoCommand
        {
            get
            {
                if (_photoCommand == null)
                {
                    _photoCommand = new RelayCommand<int>((id) => PhotoNav(id));
                }
                return _photoCommand;
            }
        }

        private void PhotoNav(int id)
        {
            Messenger.Default.Send<int, PhotoViewModel>(id);
            _navigationService.NavigateTo("PhotoView");
        }

        private ICommand _editPhotoCommand;

        public ICommand EditPhotoCommand
        {
            get
            {
                if (_editPhotoCommand == null)
                {
                    _editPhotoCommand = new RelayCommand<Tuple<int, Mode>>((tuple) => EditPhotoNav(tuple));
                }
                return _editPhotoCommand;
            }
        }

        private void EditPhotoNav(Tuple<int, Mode> tuple)
        {
            Messenger.Default.Send<Tuple<int, int, PointOfInterest, Mode>, AddEditPhotoViewModel>(new Tuple<int, int, PointOfInterest, Mode>(Trip.Id, tuple.Item1,PoiLoaded, tuple.Item2));
            _navigationService.NavigateTo("AddEditPhotoView");
        }

        #endregion

        //EXEMPLES

        public List<KeyedList<string, Picture>> GroupedPhotos
        {
            get
            {
                List<Picture> pictureList = null;
                if (PoiLoaded == null)
                    pictureList = Trip.Pictures.ToList();
                else
                    pictureList = Trip.Pictures.Where(x => (x.PointOfInterest != null) && (x.PointOfInterest == PoiLoaded)).ToList();

                var groupedPhotos =
                    from photo in pictureList
                    orderby photo.Date
                    group photo by photo.Date.ToString("m") into photosByDay
                    select new KeyedList<string, Picture>(photosByDay);

                return new List<KeyedList<string, Picture>>(groupedPhotos);
            }
        }

        #region DBMethods

        public void DeletePictures(List<object> pictureList)
        {
            DataServicePicture dsPicture = new DataServicePicture();
            foreach (Picture picture in pictureList)
            {
                Trip.Pictures.Remove(picture);
                dsPicture.DeletePicture(picture);
            }

            TripCommand.Execute(null);
        }

        public void DeletePicture(Picture picture)
        {
            DataServicePicture dsPicture = new DataServicePicture();
            Trip.Pictures.Remove(picture);
            dsPicture.DeletePicture(picture);

            TripCommand.Execute(null);
        }


        #endregion

    }
}