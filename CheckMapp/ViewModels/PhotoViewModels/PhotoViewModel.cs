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
using GalaSoft.MvvmLight.Views;
using GalaSoft.MvvmLight.Messaging;
using CheckMapp.ViewModels.TripViewModels;

namespace CheckMapp.ViewModels.PhotoViewModels
{
    public class PhotoViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        private int _selectedPictureIndex;

        public PhotoViewModel(INavigationService navigationService)
        {
            Messenger.Default.Register<int>(this,
 picture =>
 {
     DataServicePicture dsPicture = new DataServicePicture();
     this.Trip = dsPicture.getPictureById(picture).Trip;
     SelectedPictureIndex = Trip.Pictures.OrderBy(x => x.Date).ToList().FindIndex(x => x.Id == picture);
 });

            this._navigationService = navigationService;

        }

        #region Properties

        
        /// <summary>
        /// Index de la photo en cours
        /// </summary>
        public int SelectedPictureIndex
        {
            get { return _selectedPictureIndex; }
            set
            {
                _selectedPictureIndex = value;

                if (_selectedPictureIndex < 0)
                    _selectedPictureIndex = Trip.Pictures.Count - 1;

                if (_selectedPictureIndex >= Trip.Pictures.Count)
                    _selectedPictureIndex = 0;
            }
        }

        /// <summary>
        /// Voyage courant
        /// </summary>
        public Trip Trip
        {
            get;
            set;
        }

        /// <summary>
        /// L'objet photo courant
        /// </summary>
        public Picture SelectedPicture
        {
            get
            {
                return Trip.Pictures.OrderBy(x => x.Date).ToList()[SelectedPictureIndex];
            }
        }


        #endregion

        #region Buttons

        private ICommand _deletePictureCommand;
        public ICommand DeletePictureCommand
        {
            get
            {
                if (_deletePictureCommand == null)
                {
                    _deletePictureCommand = new RelayCommand(() => DeletePicture());
                }
                return _deletePictureCommand;
            }

        }

        private ICommand _editPhotoCommand;

        public ICommand EditPhotoCommand
        {
            get
            {
                if (_editPhotoCommand == null)
                {
                    _editPhotoCommand = new RelayCommand<Mode>((mode) => EditPhotoNav(mode));
                }
                return _editPhotoCommand;
            }
        }

        private void EditPhotoNav(Mode mode)
        {
            Messenger.Default.Send<Tuple<int, int, Mode>, AddEditPhotoViewModel>(new Tuple<int, int, Mode>(SelectedPicture.Trip.Id, SelectedPicture.Id, mode));
            _navigationService.NavigateTo("AddEditPhotoView");
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
            Messenger.Default.Send<int, TripViewModel>(Trip.Id);
            _navigationService.GoBack();
        }

        #endregion

        #region DBMethods

        /// <summary>
        /// Suppression d'une photo
        /// </summary>
        public void DeletePicture()
        {
            DataServicePicture dsPicture = new DataServicePicture();
            dsPicture.DeletePicture(SelectedPicture);
            Trip.Pictures.Remove(SelectedPicture);
            GoBackCommand.Execute(null);
        }


        #endregion
    }
}