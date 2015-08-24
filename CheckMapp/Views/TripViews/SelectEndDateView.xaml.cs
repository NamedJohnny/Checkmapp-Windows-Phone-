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
using CheckMapp.Model.Tables;
using CheckMapp.Resources;

namespace CheckMapp.Views.TripViews
{
    public partial class SelectEndDateView : PhoneApplicationPage
    {
        public SelectEndDateView()
        {
            InitializeComponent();
        }

        public SelectEndDateViewModel ViewModel
        {
            get
            {
                return this.DataContext as SelectEndDateViewModel;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {

            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Back)
            {
                // wait till the next UI thread tick so that the binding gets updated
                Dispatcher.BeginInvoke(() =>
                {

                    ViewModel.FinishTripCommand.Execute(null);

                    if (ViewModel.IsFormValid)
                    {
                        ViewModel.MainPageCommand.Execute(null);
                    }
                });


            }
            else if (e.NavigationMode == NavigationMode.Back && !ViewModel.IsFormValid)
            {
                ViewModel.CancelSelectDateCommand.Execute(null);
            }

            base.OnNavigatedFrom(e);
        }

        private void IconSave_Click(object sender, EventArgs e)
        {
            this.Focus();

            // wait till the next UI thread tick so that the binding gets updated
            Dispatcher.BeginInvoke(() =>
            {

                ViewModel.FinishTripCommand.Execute(null);

                if (ViewModel.IsFormValid)
                {
                    // En appelant directement la page principale on rafraichit celle-ci pour mettre a jour le panorama
                    ViewModel.MainPageCommand.Execute(null);
                }
            });


        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApplicationBar.Buttons != null)
            {
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.Save;
            }
        }
    }
}