using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CheckMapp.ViewModels.PhotoViewModels;
using CheckMapp.Resources;
using CheckMapp.ViewModels;
using CheckMapp.Model.Tables;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using CheckMapp.Utils;
using GalaSoft.MvvmLight.Messaging;
using CheckMapp.ViewModels.POIViewModels;

namespace CheckMapp.Views.PhotoViews
{
    public partial class ListPhotoView : PhoneApplicationPage
    {
        public ListPhotoView()
        {
            InitializeComponent();
        }

        public ListPhotoViewModel ViewModel
        {
            get
            {
                return this.DataContext as ListPhotoViewModel;
            }
        }

        private void IconAdd_Click(object sender, EventArgs e)
        {
            ViewModel.EditPhotoCommand.Execute(new Tuple<int, Mode>(0, Mode.add));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            var appbar = this.Resources["AppBarList"] as ApplicationBar;
            if (appbar.Buttons != null)
            {
                (appbar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.Select;
                (appbar.Buttons[1] as ApplicationBarIconButton).Text = AppResources.AddPicture;
            }

            var appbarSelect = this.Resources["AppBarListSelect"] as ApplicationBar;
            if (appbarSelect.Buttons != null)
            {
                (appbarSelect.Buttons[0] as ApplicationBarIconButton).Text = AppResources.Delete;
            }
            ApplicationBar = this.Resources["AppBarList"] as ApplicationBar;
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = (PhotoHubLLS.ItemsSource.Count > 0);


        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            PhotoHubLLS.ItemsSource = (this.DataContext as ListPhotoViewModel).GroupedPhotos;

            ViewModel.Loading = false;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back && PhotoHubLLS.IsSelectionEnabled)
            {
                PhotoHubLLS.IsSelectionEnabled = false;
                e.Cancel = true;
            }
            if (e.Uri.OriginalString.Contains("ListPOI"))
            {
                Messenger.Default.Send<int, ListPOIViewModel>(ViewModel.Trip.Id);
            }
        }

        /// <summary>
        /// Click sur les options du menu contextuel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null && ((sender as MenuItem).DataContext is Picture))
            {
                Picture pictureSelected = (sender as MenuItem).DataContext as Picture;
                switch (menuItem.Name)
                {
                    case "EditPhoto":
                        ViewModel.EditPhotoCommand.Execute(new Tuple<int, Mode>(pictureSelected.Id, Mode.edit));
                        break;
                    case "DeletePhoto":
                        if (MessageBox.Show(AppResources.ConfirmationDeletePicture, "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            ViewModel.DeletePictureCommand.Execute(pictureSelected);
                            PhotoHubLLS.ItemsSource = ViewModel.GroupedPhotos;

                            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = (PhotoHubLLS.ItemsSource.Count > 0);

                        }
                        break;
                }
            }
        }

        /// <summary>
        /// J'ai besoin de ça pour mettre à jour mon ContextMenu lorsque je reviens à un changement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            var menu = (ContextMenu)sender;
            var owner = (FrameworkElement)menu.Owner;
            if (owner.DataContext != menu.DataContext)
                menu.DataContext = owner.DataContext;
        }

        private void IconMultiSelect_Click(object sender, EventArgs e)
        {
            PhotoHubLLS.IsSelectionEnabled = !PhotoHubLLS.IsSelectionEnabled;
        }

        private void PhotoHubLLS_IsSelectionEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (PhotoHubLLS.IsSelectionEnabled)
                ApplicationBar = this.Resources["AppBarListSelect"] as ApplicationBar;
            else
                ApplicationBar = this.Resources["AppBarList"] as ApplicationBar;

            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = !PhotoHubLLS.IsSelectionEnabled;
        }

        private void IconDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(AppResources.ConfirmationDeletePictures, "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {

                ViewModel.DeletePicturesCommand.Execute(new List<object>(PhotoHubLLS.SelectedItems as IList<object>));
                PhotoHubLLS.ItemsSource = ViewModel.GroupedPhotos;
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = (PhotoHubLLS.ItemsSource.Count > 0);
            }
        }

        private void StackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhotoHubLLS.IsSelectionEnabled = false;

            if (!PhotoHubLLS.IsSelectionEnabled)
            {
                 Picture itemTapped = (sender as FrameworkElement).DataContext as Picture;
                ViewModel.PhotoCommand.Execute(itemTapped.Id);
            }
        }

        private void PhotoHubLLS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PhotoHubLLS.IsSelectionEnabled)
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = (PhotoHubLLS.SelectedItems.Count > 0);
        }
    }
}