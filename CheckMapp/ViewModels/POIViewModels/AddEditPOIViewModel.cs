using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.ComponentModel;
using CheckMapp.Model.Tables;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Windows.Controls;
using System.Windows;
using CheckMapp.Model.DataService;
using FluentValidation;
using CheckMapp.Utils.Validations;
using Utility = CheckMapp.Utils.Utility;
using CheckMapp.Utils.EditableObject;
using GalaSoft.MvvmLight.Messaging;
using System;
using GalaSoft.MvvmLight.Views;
using CheckMapp.ViewModels.TripViewModels;

namespace CheckMapp.ViewModels.POIViewModels
{
    public class AddEditPOIViewModel : ViewModelBase
    {
        private ICommand _addPOICommand;
        private INavigationService _navigationService;

        // Used for validate the form
        private IValidator<PointOfInterest> _validator;

        /// <summary>
        /// Initializes a new instance of the AddPOIViewModel class.
        /// </summary>
        public AddEditPOIViewModel(INavigationService navigationservice)
        {
            Messenger.Default.Register<Tuple<int, int, Mode>>(this,
 tuple =>
 {
     this.Mode = tuple.Item3;

     if (this.Mode == Mode.add)
     {
         PointOfInterest = new Model.Tables.PointOfInterest();
         DataServiceTrip dsTrip = new DataServiceTrip();
         PointOfInterest.Trip = dsTrip.getTripById(tuple.Item1);
     }
     else
     {
         PointOfInterest = GetPOIInDB(tuple.Item2);
     }

     EditableObject = new Caretaker<PointOfInterest>(this.PointOfInterest);
     EditableObject.BeginEdit();

     InitialiseValidator();
 });

            Messenger.Default.Register<PointOfInterest>(this,
 poi =>
 {
     PointOfInterest = poi;

     
 });

            Messenger.Default.Register<Tuple<PointOfInterest,Mode>>(this,
tuple =>
{
    this.Mode = tuple.Item2;
    PointOfInterest = tuple.Item1;

    EditableObject = new Caretaker<PointOfInterest>(this.PointOfInterest);
    EditableObject.BeginEdit();

    InitialiseValidator();
});

            this._navigationService = navigationservice;

        }

        private void InitialiseValidator()
        {
            _validator = new ValidatorFactory().GetValidator<PointOfInterest>();
        }

        /// <summary>
        /// Mon objet editable, nécessaire pour annuler les changements
        /// </summary>
        private Caretaker<PointOfInterest> EditableObject { get; set; }
        /// <summary>
        /// Ajout d'un point d'intérêt
        /// </summary>
        public ICommand AddPOICommand
        {
            get
            {
                if (_addPOICommand == null)
                {
                    _addPOICommand = new RelayCommand(() => AddPOI());
                }
                return _addPOICommand;
            }

        }

        private ICommand _cancelPOICommand;
        public ICommand CancelPOICommand
        {
            get
            {
                if (_cancelPOICommand == null)
                {
                    _cancelPOICommand = new RelayCommand(() => CancelPOI());
                }
                return _cancelPOICommand;
            }
        }

        #region Properties

        private PointOfInterest _pointOfInterest;

        /// <summary>
        /// Le point d'intérêt courant
        /// </summary>
        public PointOfInterest PointOfInterest
        {
            get
            {
                return _pointOfInterest;
            }
            set
            {
                _pointOfInterest = value;
            }
        }

        private bool _isFormValid;

        /// <summary>
        /// Si la form est valid
        /// </summary>
        public bool IsFormValid
        {
            get { return _isFormValid; }
            set
            {
                _isFormValid = value;
            }
        }

        /// <summary>
        /// Mode édition ou ajout
        /// </summary>
        public Mode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// Le nom de notre point d'intérêt
        /// </summary>
        public string PoiName
        {
            get { return PointOfInterest.Name; }
            set
            {
                PointOfInterest.Name = value;
                RaisePropertyChanged("PoiName");
            }
        }

        /// <summary>
        /// La localisation en texte
        /// </summary>
        public string PoiLocation
        {
            get
            {
                return PointOfInterest.Location;
            }
            set
            {
                PointOfInterest.Location = value;
                RaisePropertyChanged("PoiLocation");
            }
        }

        /// <summary>
        /// La longitude
        /// </summary>
        public double Latitude
        {
            get { return PointOfInterest.Latitude; }
            set
            {
                PointOfInterest.Latitude = value;
            }
        }

        /// <summary>
        /// La latitude
        /// </summary>
        public double Longitude
        {
            get { return PointOfInterest.Longitude; }
            set
            {
                PointOfInterest.Longitude = value;
            }
        }

        private ICommand _selectTypeCommand;

        public ICommand SelectTypeCommand
        {
            get
            {
                if (_selectTypeCommand == null)
                {
                    _selectTypeCommand = new RelayCommand<Tuple<int, Mode>>((tuple) => SelectTypeNav());
                }
                return _selectTypeCommand;
            }
        }

        private void SelectTypeNav()
        {
            Messenger.Default.Send<PointOfInterest, SelectTypePOIViewModel>(PointOfInterest);
            _navigationService.NavigateTo("SelectTypePOIView");
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
            Messenger.Default.Send<int, ListPOIViewModel>(PointOfInterest.Trip.Id);
            Messenger.Default.Send<int, TripViewModel>(PointOfInterest.Trip.Id);
            _navigationService.GoBack();
        }

        #endregion

        #region DBMethods

        /// <summary>
        /// Ajout d'un point d'intérêt
        /// </summary>
        public void AddPOI()
        {
            if (ValidationErrorsHandler.IsValid(_validator, PointOfInterest))
            {
                _isFormValid = true;
                // Adding a poi
                if (Mode == Mode.add || Mode == Mode.addFromExisting)
                    AddPoiInDB();
                else
                    UpdateExistingPOI();

                EditableObject.EndEdit();
            }
            else
            {
                _isFormValid = false;
            }
        }

        public void CancelPOI()
        {
            Messenger.Default.Send<int, ListPOIViewModel>(PointOfInterest.Trip.Id);
            Messenger.Default.Send<int, TripViewModel>(PointOfInterest.Trip.Id);

            if (Mode == ViewModels.Mode.add || Mode == ViewModels.Mode.addFromExisting)
            {
                PointOfInterest.Trip.PointsOfInterests.Remove(PointOfInterest);
                PointOfInterest.Trip = null;
            }

            EditableObject.CancelEdit();
        }

        /// <summary>
        /// Ajouter le point
        /// </summary>
        public void AddPoiInDB()
        {
            PointOfInterest.Trip.PointsOfInterests.Add(PointOfInterest);

            DataServicePoi dsPoi = new DataServicePoi();
            dsPoi.addPoi(PointOfInterest);
        }

        /// <summary>
        /// Mettre à jour le point
        /// </summary>
        private void UpdateExistingPOI()
        {
            DataServicePoi dsPOI = new DataServicePoi();
            dsPOI.UpdatePoi(PointOfInterest);
        }
        private PointOfInterest GetPOIInDB(int poiId)
        {
            DataServicePoi dsPoi = new DataServicePoi();
            return dsPoi.getPOIById(poiId);
        }


        #endregion

    }
}