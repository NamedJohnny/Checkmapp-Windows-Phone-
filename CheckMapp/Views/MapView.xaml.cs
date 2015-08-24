using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Maps.Controls;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Maps.Toolkit;
using CheckMapp.ViewModels;
using System.Collections.ObjectModel;
using System.Collections;
using CheckMapp.Model.Tables;

namespace CheckMapp.Pages
{
    public partial class MapView : PhoneApplicationPage
    {
        public MapView()
        {
            InitializeComponent();
        }

        public MapViewModel ViewModel
        {
            get
            {
                return this.DataContext as MapViewModel;
            }
        }

        private void Pushpin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Trip clickTrip = (sender as Pushpin).DataContext as Trip;
            ViewModel.TripCommand.Execute(clickTrip.Id);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            ObservableCollection<DependencyObject> children = MapExtensions.GetChildren(MyMap);
            var obj = children.FirstOrDefault(x => x.GetType() == typeof(MapItemsControl)) as MapItemsControl;
            if (obj.ItemsSource == null)
            {
                obj.ItemsSource = ViewModel.TripPoints;
            }
        }


    }
}