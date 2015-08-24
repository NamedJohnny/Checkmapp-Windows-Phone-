using CheckMapp.Model.Tables;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CheckMapp.ViewModels.POIViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SelectTypePOIViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        /// <summary>
        /// Initializes a new instance of the SelectTypePOIViewModel class.
        /// </summary>
        public SelectTypePOIViewModel(INavigationService navigationService)
        {
            Messenger.Default.Register<PointOfInterest>(this,
poi =>
{
    PointOfInterest = poi;
});
            POITypeList = new ObservableCollection<POIType>();
            foreach (POIType item in Enum.GetValues(typeof(POIType)))
            {
                POITypeList.Add(item);
            }

            this._navigationService = navigationService;
        }

        public ObservableCollection<POIType> POITypeList
        {
            get;
            set;
        }

        public PointOfInterest PointOfInterest
        {
            get;
            set;
        }


        private POIType _selectedItem;

        public POIType SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                PointOfInterest.Type = value;
                RaisePropertyChanged("SelectedItem");
            }
        }

        private ICommand _addEditPOICommand;

        public ICommand AddEditPOICommand
        {
            get
            {
                if (_addEditPOICommand == null)
                {
                    _addEditPOICommand = new RelayCommand(() => AddEditPOI());
                }
                return _addEditPOICommand;
            }
        }

        private void AddEditPOI()
        {
            //Messenger.Default.Send<PointOfInterest, AddEditPOIViewModel>(PointOfInterest);
            _navigationService.GoBack();
        }


    }
}