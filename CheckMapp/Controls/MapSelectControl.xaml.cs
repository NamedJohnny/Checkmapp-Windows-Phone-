using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CheckMapp.Utils;
using Microsoft.Phone.Maps.Controls;
using CheckMapp.Resources;
using System.Threading;
using System.Device.Location;
using Microsoft.Phone.Maps.Services;

namespace CheckMapp.Controls
{
    public partial class MapSelectControl : UserControl
    {
        public MapSelectControl()
        {
            InitializeComponent();

            this.PoiTextBox.IsEnabled = true;
            this.btn_place.Visibility = Visibility.Visible;
            btn_place.IsEnabled = !String.IsNullOrEmpty(PoiTextBox.Text);
        }

        public static readonly DependencyProperty CompleteAdressProperty =
     DependencyProperty.Register("CompleteAdress", typeof(bool), typeof(MapSelectControl), null);

        public bool CompleteAdress
        {
            get { return (bool)GetValue(CompleteAdressProperty); }
            set
            {
                SetValue(CompleteAdressProperty, value);
            }
        }

        public static readonly DependencyProperty LongitudeProperty =
     DependencyProperty.Register("Longitude", typeof(double), typeof(MapSelectControl), null);

        public double Longitude
        {
            get { return (double)GetValue(LongitudeProperty); }
            set
            {
                SetValue(LongitudeProperty, value);
            }
        }


        public static readonly DependencyProperty LatitudeProperty =
     DependencyProperty.Register("Latitude", typeof(double), typeof(MapSelectControl), null);

        public double Latitude
        {
            get { return (double)GetValue(LatitudeProperty); }
            set
            {
                SetValue(LatitudeProperty, value);
            }
        }

        public static readonly DependencyProperty LocationProperty =
DependencyProperty.Register("PoiLocation", typeof(string), typeof(MapSelectControl), null);

        public string PoiLocation
        {
            get { return (string)GetValue(LocationProperty); }
            set
            {
                SetValue(LocationProperty, value);
            }
        }


        private void PoiTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            btn_place.IsEnabled = !String.IsNullOrEmpty(PoiTextBox.Text);
        }

        private void btn_place_Click(object sender, RoutedEventArgs e)
        {
            AfficherCarte(this.PoiTextBox, this.myMap);
        }

        private async void Map_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            await Utility.AddLocation(this.myMap, this.PoiTextBox, e, 0.0, 0.0, CompleteAdress);
            if (this.myMap.Layers != null && this.myMap.Layers.Count > 0)
            {
                MapLayer layer = this.myMap.Layers.FirstOrDefault();
                Latitude = layer[0].GeoCoordinate.Latitude;
                Longitude = layer[0].GeoCoordinate.Longitude;
            }
        }

        private void AfficherCarte(PhoneTextBox myTextBox, Microsoft.Phone.Maps.Controls.Map myMap)
        {
            GeocodeQuery query = new GeocodeQuery()
            {
                GeoCoordinate = new GeoCoordinate(0, 0),
                SearchTerm = myTextBox.Text
            };
            query.QueryCompleted += query_QueryCompleted;
            query.QueryAsync();
        }

        async void query_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            if (e.Result.Count == 0)
            {
                MessageBox.Show(string.Format(AppResources.InvalideSearch, this.PoiTextBox.Text), AppResources.Warning, MessageBoxButton.OK);
                return;
            }
            GeoCoordinate coord = new GeoCoordinate();
            foreach (var item in e.Result)
            {
                coord = item.GeoCoordinate;
            }
            // CoordinateList[0] = latitude, CoordinateList[1] = longitude
            await Utility.AddLocation(myMap, this.PoiTextBox, null, coord.Latitude, coord.Longitude, CompleteAdress);

            Latitude = coord.Latitude;
            Longitude = coord.Longitude;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // No need to wait for the result, we hide the warning
#pragma warning disable 4014
            Utility.AddLocation(this.myMap, this.PoiTextBox, null, Latitude, Longitude, CompleteAdress);
            Thread.Sleep(500);
        }
    }
}
