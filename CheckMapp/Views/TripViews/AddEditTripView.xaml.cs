using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CheckMapp.ViewModels.TripViewModels;
using CheckMapp.Resources;
using CheckMapp.ViewModels;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using CheckMapp.Utils;
using CheckMapp.Model.Tables;
using System.Device.Location;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps.Controls;
using System.Threading.Tasks;
using Microsoft.Phone.UserData;
using Windows.ApplicationModel.Appointments;
using Windows.Storage;

namespace CheckMapp.Views.TripViews
{
    public partial class AddEditTripView : PhoneApplicationPage
    {
        public AddEditTripView()
        {
            InitializeComponent();
            LoadPage();
        }

        public AddEditTripViewModel ViewModel
        {
            get
            {
                return this.DataContext as AddEditTripViewModel;
            }
        }

        /// <summary>
        /// On assigne les titres des boutons au démarrage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApplicationBar.Buttons != null)
            {
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.Save;
                (ApplicationBar.Buttons[1] as ApplicationBarIconButton).Text = AppResources.Cancel;
            }
        }

        /// <summary>
        /// Sauvegarder le voyage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconSave_Click(object sender, EventArgs e)
        {
            this.Focus();
              
            Dispatcher.BeginInvoke(() =>
            {

                if (ViewModel.Trip.MainPictureData == null)
                    {
                        //Si l'usager ne met pas d'image on en met une par défaut
                        BitmapImage logo = new BitmapImage();
                        logo.UriSource = new Uri(@"/Assets/Logo.png", UriKind.Relative);
                        logo.CreateOptions = BitmapCreateOptions.BackgroundCreation;
                        logo.ImageOpened += logo_ImageOpened;
                    }
                    else
                    {
                        ViewModel.AddEditTripCommand.Execute(null);
                        if (ViewModel.IsFormValid)
                        {
                            // En appelant directement la page principale on rafraichit celle-ci pour mettre a jour la liste des voyages
                            ViewModel.GoBackCommand.Execute(null);
                        }
                    }
                
            });

        }

        void logo_ImageOpened(object sender, RoutedEventArgs e)
        {

            ViewModel.Trip.MainPictureData = Utils.Utility.ConvertToBytes(sender as BitmapImage);

            ViewModel.AddEditTripCommand.Execute(null);

            if (ViewModel.IsFormValid)
            {
                // En appelant directement la page principale on rafraichit celle-ci pour mettre a jour la liste des voyages
                ViewModel.GoBackCommand.Execute(null);
            }
        }

        /// <summary>
        /// Annuler le voyage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconCancel_Click(object sender, EventArgs e)
        {
            ViewModel.GoBackCommand.Execute(null);
        }

        private void HubTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhotoChooserTask photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += photoChooserTask_Completed;
            photoChooserTask.ShowCamera = true;
            photoChooserTask.PixelHeight = 500;
            photoChooserTask.PixelWidth = 500;
            photoChooserTask.Show();
        }

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                ViewModel.MainImage = Utils.Utility.ReadFully(e.ChosenPhoto);
                hubTile.Source = Utility.ByteArrayToImage(ViewModel.MainImage, false);
            }

        }

        private void StackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            AddressChooserTask phoneNumberChooserTask = new AddressChooserTask();
            phoneNumberChooserTask.Completed += phoneNumberChooserTask_Completed;
            phoneNumberChooserTask.Show();
        }

        void phoneNumberChooserTask_Completed(object sender, AddressResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                ViewModel.FriendList.Add(e.DisplayName);
                FriendLLS.ItemsSource = null;
                FriendLLS.ItemsSource = (this.DataContext as AddEditTripViewModel).FriendList;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //On annule les changeemnts si l'usager fait BACK
            if (e.NavigationMode == NavigationMode.Back && !ViewModel.IsFormValid)
                ViewModel.CancelTripCommand.Execute(null);

            base.OnNavigatedFrom(e);
        }

        private void DeleteFriend_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null && ((sender as MenuItem).DataContext is string))
            {
                string selected = ((sender as MenuItem).DataContext as string);
                ViewModel.FriendList.Remove(selected);
                FriendLLS.ItemsSource = null;
                FriendLLS.ItemsSource = ViewModel.FriendList;
            }
        }

        private void ContextMenuNote_Opened(object sender, RoutedEventArgs e)
        {
            var menu = (ContextMenu)sender;
            var owner = (FrameworkElement)menu.Owner;
            if (owner.DataContext != menu.DataContext)
                menu.DataContext = owner.DataContext;
        }

        private void LoadPage()
        {
            if (ViewModel.Mode == Mode.add)
                TitleTextblock.Text = AppResources.AddTrip.ToLower();
            else
            {
                TitleTextblock.Text = AppResources.EditTrip.ToLower();
            }
        }


    }
}