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
using Microsoft.Phone.Tasks;
using CheckMapp.ViewModels;
using CheckMapp.Utils;
using System.Windows.Media.Imaging;
using System.IO;
using CheckMapp.Model.Tables;

namespace CheckMapp.Views.TripViews
{
    public partial class TripView : PhoneApplicationPage
    {
        public TripView()
        {
            InitializeComponent();
        }

        public TripViewModel ViewModel
        {
            get
            {
                return this.DataContext as TripViewModel;
            }
        }

        private void IconButtonAddMedia_Click(object sender, EventArgs e)
        {
            ViewModel.AddPhotoCommand.Execute(new Tuple<int, Mode>(0, Mode.add));
        }

        private void IconButtonAddNotes_Click(object sender, EventArgs e)
        {
            ViewModel.AddNoteCommand.Execute(new Tuple<int, Mode>(0, Mode.add));
        }

        private void IconButtonAddPOI_Click(object sender, EventArgs e)
        {
            ViewModel.AddPOICommand.Execute(new Tuple<int, Mode>(0, Mode.add));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApplicationBar.Buttons != null && ApplicationBar.MenuItems != null)
            {
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.AddPicture;
                (ApplicationBar.Buttons[1] as ApplicationBarIconButton).Text = AppResources.AddNote;
                (ApplicationBar.Buttons[2] as ApplicationBarIconButton).Text = AppResources.AddPOI;

                (ApplicationBar.MenuItems[1] as ApplicationBarMenuItem).Text = AppResources.Delete;
                (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = AppResources.Edit;
                (ApplicationBar.MenuItems[2] as ApplicationBarMenuItem).Text = AppResources.FinishTrip;

                (ApplicationBar.MenuItems[2] as ApplicationBarMenuItem).IsEnabled = ViewModel.Trip.IsActif;
            }
        }

        private void btnNote_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ListNoteCommand.Execute(null);
        }

        private void btnPOI_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ListPOICommand.Execute(null);
        }

        private void btnStats_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StatsCommand.Execute(null);
        }

        private void btnPhoto_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ListPhotoCommand.Execute(null);
        }

        private void EditTrip_Click(object sender, EventArgs e)
        {
            ViewModel.EditTripCommand.Execute(null);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void FinisTrip_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(AppResources.ConfirmFinishTrip, "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ViewModel.SelectEndDateCommand.Execute(null);
            }
        }

        private void DeleteTrip_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(AppResources.ConfirmationDeleteTrip, "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                var vm = DataContext as TripViewModel;
                if (vm != null)
                {
                    vm.DeleteTripCommand.Execute(vm.Trip);
                    ViewModel.MainPageCommand.Execute(null);
                }
            }
        }
    }
}