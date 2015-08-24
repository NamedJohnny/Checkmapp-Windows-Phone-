using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CheckMapp.ViewModels;
using CheckMapp.Model.Tables;
using Windows.Devices.Geolocation;
using CheckMapp.Resources;
using Microsoft.Phone.Maps.Controls;
using System.Collections.ObjectModel;
using Microsoft.Phone.Maps.Toolkit;
using System.Collections;
using System.Windows.Media;

namespace CheckMapp.Views
{
    public partial class StatisticView : PhoneApplicationPage
    {

        public StatisticView()
        {
            InitializeComponent();
        }

        public StatisticViewModel ViewModel
        {
            get
            {
                return this.DataContext as StatisticViewModel;
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Trip currentTrip = ViewModel.Trip;
            if (currentTrip.IsActif)
                EndStack.Visibility = Visibility.Collapsed;
            else
                EndStack.Visibility = Visibility.Visible;
            //Bind les POI a la map
            ObservableCollection<DependencyObject> children = MapExtensions.GetChildren(statsMap);
            var obj = children.FirstOrDefault(x => x.GetType() == typeof(MapItemsControl)) as MapItemsControl;
            if (obj.ItemsSource != null)
            {
                (obj.ItemsSource as IList).Clear();
                obj.ItemsSource = null;
            }
            obj.ItemsSource = (ViewModel.PointOfInterestList);

            //Ajout du départ
            MapLayer layer1 = new MapLayer();
            Pushpin pushpin1 = new Pushpin();
            pushpin1.GeoCoordinate = currentTrip.CoordinateDeparture;
            pushpin1.Background = new SolidColorBrush(Color.FromArgb(255, 105, 105, 105));
            pushpin1.Content = AppResources.AddTripDeparture;
            MapOverlay overlay1 = new MapOverlay();
            overlay1.Content = pushpin1;
            overlay1.GeoCoordinate = currentTrip.CoordinateDeparture;
            overlay1.PositionOrigin = new Point(0, 1);
            layer1.Add(overlay1);
            statsMap.Layers.Add(layer1);

            //Ajout de la destination
            MapLayer layer2 = new MapLayer();
            Pushpin pushpin2 = new Pushpin();
            pushpin2.GeoCoordinate = currentTrip.CoordinateDestination;
            pushpin2.Content = AppResources.AddTripArrival;
            pushpin2.Background = new SolidColorBrush(Color.FromArgb(255, 105, 105, 105));
            MapOverlay overlay2 = new MapOverlay();
            overlay2.Content = pushpin2;
            overlay2.GeoCoordinate = currentTrip.CoordinateDestination;
            overlay2.PositionOrigin = new Point(0, 1);
            layer2.Add(overlay2);
            statsMap.Layers.Add(layer2);
        }

        private void statsMap_Loaded(object sender, RoutedEventArgs e)
        {
            System.Threading.Thread.Sleep(500);

            var poiList = ViewModel.PointOfInterestList;
            if (poiList.Count > 0)
            {
                var bounds = new LocationRectangle(
                    poiList.Max((p) => p.Latitude),
                    poiList.Min((p) => p.Longitude),
                    poiList.Min((p) => p.Latitude),
                    poiList.Max((p) => p.Longitude));
                statsMap.SetView(bounds);
            }
        }
    }
}