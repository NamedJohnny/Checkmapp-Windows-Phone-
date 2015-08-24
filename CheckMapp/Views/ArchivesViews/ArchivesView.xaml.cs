using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CheckMapp.ViewModels.ArchivesViewModels;
using CheckMapp.Resources;
using CheckMapp.Model.Tables;
using CheckMapp.ViewModel;

namespace CheckMapp.Views.ArchivesViews
{
    public partial class ArchivesView : UserControl
    {
        public ArchivesView()
        {
            InitializeComponent();
        }

        public ArchivesViewModel ViewModel
        {
            get
            {
                return this.DataContext as ArchivesViewModel;
            }
        }

        private void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                Trip tripSelected = ((sender as MenuItem).DataContext as Trip);
                switch (menuItem.Name)
                {
                    case "Delete":
                        if (MessageBox.Show(AppResources.ConfirmationDeleteTrip, "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            MainViewModel myModel = (((PhoneApplicationFrame)Application.Current.RootVisual).Content as MainPage).DataContext as MainViewModel;
                            ViewModel.ArchiveTripList.Remove(tripSelected);
                            ViewModel.DeleteTripCommand.Execute(tripSelected);
                            listArchiveTrips.ItemsSource = ViewModel.ArchiveTripList;
                            myModel.TripList.Remove(tripSelected);
                        }
                        break;
                }
            }
        }

        private void StackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ViewModel.TripCommand.Execute(((sender as FrameworkElement).DataContext as Trip).Id);
        }

    }
}
