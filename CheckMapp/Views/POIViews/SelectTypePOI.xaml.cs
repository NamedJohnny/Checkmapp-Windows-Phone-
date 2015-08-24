using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CheckMapp.ViewModels.POIViewModels;
using CheckMapp.ViewModels;

namespace CheckMapp.Views.POIViews
{
    public partial class SelectTypePOI : PhoneApplicationPage
    {
        public SelectTypePOI()
        {
            InitializeComponent();
        }

        public SelectTypePOIViewModel ViewModel
        {
            get
            {
                return this.DataContext as SelectTypePOIViewModel;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.AddEditPOICommand.Execute(null);
        }
    }
}