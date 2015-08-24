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
using CheckMapp.ViewModels;
using CheckMapp.Resources;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using CheckMapp.Model.Tables;
using System.IO;
using CheckMapp.Utils;

namespace CheckMapp.Views.PhotoViews
{
    public partial class AddEditPhotoView : PhoneApplicationPage
    {
        public AddEditPhotoView()
        {
            InitializeComponent();
            LoadPage();
        }

        public AddEditPhotoViewModel ViewModel
        {
            get
            {
                return this.DataContext as AddEditPhotoViewModel;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            Uri destination = e.Uri;

            if (!destination.OriginalString.Contains("DatePickerPage.xaml"))
            {
                if (hubTile.Source != null)
                    hubTile.Source.ClearValue(BitmapImage.UriSourceProperty);
            }

            //On annule les changeemnts si l'usager fait BACK
            if (e.NavigationMode == NavigationMode.Back && !ViewModel.IsFormValid)
                ViewModel.CancelPhotoCommand.Execute(null);

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApplicationBar.Buttons != null)
            {
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.Save;
                (ApplicationBar.Buttons[1] as ApplicationBarIconButton).Text = AppResources.Cancel;
            }

            if (ViewModel.POISelected != null)
                POIControl.chkNoPOI.IsChecked = false;
        }

        private void IconSave_Click(object sender, EventArgs e)
        {
            this.Focus();

            // wait till the next UI thread tick so that the binding gets updated
            Dispatcher.BeginInvoke(() =>
            {
                ViewModel.AddEditPhotoCommand.Execute(null);
                if (ViewModel.IsFormValid)
                    ViewModel.GoBackCommand.Execute(null);
            });

        }

        private void IconCancel_Click(object sender, EventArgs e)
        {
            ViewModel.GoBackCommand.Execute(null);
        }

        private void HubTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhotoChooserTask photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += photoChooserTask_Completed;
            photoChooserTask.ShowCamera = true;
            photoChooserTask.Show();
        }

        private void LoadPage()
        {
            //Assigne le titre de la page
            if (ViewModel.Mode == Mode.add)
                TitleTextblock.Text = AppResources.AddPicture.ToLower();
            else if (ViewModel.Mode == Mode.edit)
                TitleTextblock.Text = AppResources.EditPicture.ToLower();
        }

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                BitmapImage img = new BitmapImage();
                try
                {
                    img.SetSource(e.ChosenPhoto);
                    hubTile.Source = img;

                    ViewModel.ImageSource = Utils.Utility.ConvertToBytes(img);
                }
                catch (Exception ex) {
                    MessageBox.Show(AppResources.CannotChoosePic, AppResources.Warning,MessageBoxButton.OK);
                }
            }
        }
    }
}