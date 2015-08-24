/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:CheckMapp.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using CheckMapp.Model;
using GalaSoft.MvvmLight.Views;
using CheckMapp.ViewModels.PhotoViewModels;
using CheckMapp.ViewModels.ArchivesViewModels;
using CheckMapp.ViewModels.NoteViewModels;
using CheckMapp.ViewModels.POIViewModels;
using CheckMapp.ViewModels.SettingsViewModels;
using CheckMapp.ViewModels.TripViewModels;
using CheckMapp.ViewModels;
using System;

namespace CheckMapp.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            var navigationService = CreateNavigationService();
            SimpleIoc.Default.Register<INavigationService>(() => navigationService);
            SimpleIoc.Default.Register<IDialogService, DialogService>();

            SimpleIoc.Default.Register<ArchivesViewModel>(true);
            SimpleIoc.Default.Register<TimelineViewModel>(true);
            SimpleIoc.Default.Register<AddEditNoteViewModel>(true);
            SimpleIoc.Default.Register<NoteViewModel>(true);
            SimpleIoc.Default.Register<ListNoteViewModel>(true);
            SimpleIoc.Default.Register<AddEditPhotoViewModel>(true);
            SimpleIoc.Default.Register<PhotoViewModel>(true);
            SimpleIoc.Default.Register<ListPhotoViewModel>(true);
            SimpleIoc.Default.Register<AddEditPOIViewModel>(true);
            SimpleIoc.Default.Register<ListPOIViewModel>(true);
            SimpleIoc.Default.Register<SelectTypePOIViewModel>(true);
            SimpleIoc.Default.Register<SettingsViewModel>(true);
            SimpleIoc.Default.Register<AddEditTripViewModel>(true);
            SimpleIoc.Default.Register<TripViewModel>(true);
            SimpleIoc.Default.Register<CurrentViewModel>(true);
            SimpleIoc.Default.Register<SelectEndDateViewModel>(true);
            SimpleIoc.Default.Register<MainViewModel>(true);
            SimpleIoc.Default.Register<MapViewModel>(true);
            SimpleIoc.Default.Register<StatisticViewModel>(true);
            SimpleIoc.Default.Register<DashboardViewModel>(true);
        }

        private INavigationService CreateNavigationService()
        {
            var navigationService = new NavigationService();
            navigationService.Configure("ArchivesView", new Uri("/Views/ArchivesViews/ArchivesView.xaml", UriKind.Relative));
            navigationService.Configure("TimelineView", new Uri("/Views/ArchivesViews/TimelineView.xaml", UriKind.Relative));
            navigationService.Configure("AddEditNoteView", new Uri("/Views/NoteViews/AddEditNoteView.xaml", UriKind.Relative));
            navigationService.Configure("NoteView", new Uri("/Views/NoteViews/NoteView.xaml", UriKind.Relative));
            navigationService.Configure("ListNoteView", new Uri("/Views/NoteViews/ListNoteView.xaml", UriKind.Relative));
            navigationService.Configure("AddEditPhotoView", new Uri("/Views/PhotoViews/AddEditPhotoView.xaml", UriKind.Relative));
            navigationService.Configure("PhotoView", new Uri("/Views/PhotoViews/PhotoView.xaml", UriKind.Relative));
            navigationService.Configure("ListPhotoView", new Uri("/Views/PhotoViews/ListPhotoView.xaml", UriKind.Relative));
            navigationService.Configure("AddEditPOIView", new Uri("/Views/POIViews/AddEditPOIView.xaml", UriKind.Relative));
            navigationService.Configure("ListPOIView", new Uri("/Views/POIViews/ListPOIView.xaml", UriKind.Relative));
            navigationService.Configure("SelectTypePOIView", new Uri("/Views/POIViews/SelectTypePOI.xaml", UriKind.Relative));
            navigationService.Configure("SettingsView", new Uri("/Views/SettingsViews/SettingsView.xaml", UriKind.Relative));
            navigationService.Configure("AddEditTripView", new Uri("/Views/TripViews/AddEditTripView.xaml", UriKind.Relative));
            navigationService.Configure("TripView", new Uri("/Views/TripViews/TripView.xaml", UriKind.Relative));
            navigationService.Configure("CurrentView", new Uri("/Views/TripViews/CurrentView.xaml", UriKind.Relative));
            navigationService.Configure("SelectEndDateView", new Uri("/Views/TripViews/SelectEndDateView.xaml", UriKind.Relative));
            navigationService.Configure("MainView", new Uri("/MainPage.xaml", UriKind.Relative));
            navigationService.Configure("MapView", new Uri("/Views/MapView.xaml", UriKind.Relative));
            navigationService.Configure("StatisticView", new Uri("/Views/StatisticView.xaml", UriKind.Relative));
            navigationService.Configure("DashboardView", new Uri("/Views/DashboardView.xaml", UriKind.Relative));
            return navigationService;
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        public MainViewModel MainViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public TimelineViewModel TimelineViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<TimelineViewModel>();
            }
        }

        public ArchivesViewModel ArchivesViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ArchivesViewModel>();
            }
        }

        public AddEditNoteViewModel AddEditNoteViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AddEditNoteViewModel>();
            }
        }

        public NoteViewModel NoteViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<NoteViewModel>();
            }
        }

        public ListNoteViewModel ListNoteViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ListNoteViewModel>();
            }
        }

        public AddEditPhotoViewModel AddEditPhotoViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AddEditPhotoViewModel>();
            }
        }

        public ListPhotoViewModel ListPhotoViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ListPhotoViewModel>();
            }
        }

        public PhotoViewModel PhotoViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PhotoViewModel>();
            }
        }

        public AddEditPOIViewModel AddEditPOIViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AddEditPOIViewModel>();
            }
        }

        public ListPOIViewModel ListPOIViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ListPOIViewModel>();
            }
        }

        public SelectTypePOIViewModel SelectTypePOIViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SelectTypePOIViewModel>();
            }
        }

        public SettingsViewModel SettingsViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }

        public TripViewModel TripViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<TripViewModel>();
            }
        }

        public AddEditTripViewModel AddEditTripViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AddEditTripViewModel>();
            }
        }

        public CurrentViewModel CurrentViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CurrentViewModel>();
            }
        }

        public SelectEndDateViewModel SelectEndDateViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SelectEndDateViewModel>();
            }
        }

        public MapViewModel MapViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MapViewModel>();
            }
        }

        public StatisticViewModel StatisticViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StatisticViewModel>();
            }
        }

        public DashboardViewModel DashboardViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DashboardViewModel>();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}