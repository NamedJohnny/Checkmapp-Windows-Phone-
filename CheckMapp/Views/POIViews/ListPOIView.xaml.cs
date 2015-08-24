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
using CheckMapp.Resources;
using CheckMapp.Model.Tables;
using System.Collections.ObjectModel;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using System.Device.Location;
using System.Collections;
using CheckMapp.ViewModels;
using Microsoft.Phone.Tasks;
using Windows.Devices.Geolocation;
using System.Windows.Media;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Runtime.Serialization;
using CheckMapp.Utils;
using System.Globalization;
using System.Threading;
using System.Reflection;

namespace CheckMapp.Views.POIViews
{
    public partial class ListPOIView : PhoneApplicationPage
    {
        private string currentLatitude = string.Empty;
        private string currentLongitude = string.Empty;
        private bool locationError;

        public ListPOIView()
        {
            InitializeComponent();
        }

        public ListPOIViewModel ViewModel
        {
            get
            {
                return this.DataContext as ListPOIViewModel;
            }
        }

        private void loadData()
        {
            ////Bind les POI a la map
            ObservableCollection<DependencyObject> children = MapExtensions.GetChildren(MyMap);
            var obj = children.FirstOrDefault(x => x.GetType() == typeof(MapItemsControl)) as MapItemsControl;
            if (obj.ItemsSource != null)
            {
                removeTempMapLayer();
                (obj.ItemsSource as IList).Clear();
                obj.ItemsSource = null;
            }
            obj.ItemsSource = ViewModel.PointOfInterestList;
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

        private async void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null && ((sender as MenuItem).DataContext is PointOfInterest))
            {
                PointOfInterest poiSelected = (sender as MenuItem).DataContext as PointOfInterest;
                switch (menuItem.Name)
                {
                    case "POIGet":
                        try
                        {
                            ViewModel.Loading = true;
                            BingMapsDirectionsTask bingMap = new BingMapsDirectionsTask();
                            // Get my current location.
                            Geolocator myGeolocator = new Geolocator();
                            Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
                            Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
                            bingMap.Start = new LabeledMapLocation();
                            bingMap.End = new LabeledMapLocation();
                            bingMap.Start.Label = AppResources.YourPosition;
                            bingMap.Start.Location = new GeoCoordinate(myGeocoordinate.Point.Position.Latitude, myGeocoordinate.Point.Position.Longitude);
                            bingMap.End.Location = poiSelected.Coordinate;
                            bingMap.End.Label = poiSelected.Name;
                            bingMap.Show();
                            ViewModel.Loading = false;
                        }
                        catch (Exception)
                        {
                            // the app does not have the right capability or the location master switch is off 
                            MessageBox.Show(AppResources.LocationError, AppResources.Warning, MessageBoxButton.OK);
                        }
                        break;
                    case "POIShare":
                        ShareStatusTask status = new ShareStatusTask();
                        status.Status = String.Format(AppResources.IamHere, poiSelected.Name);
                        status.Show();
                        break;
                    case "POIPictures":
                        ViewModel.ListPhotoCommand.Execute(poiSelected);
                        break;
                    case "POINotes":
                        ViewModel.ListNoteCommand.Execute(poiSelected);
                        break;
                    case "EditPoi":
                        ViewModel.EditPOICommand.Execute(new Tuple<int, Mode>(poiSelected.Id, Mode.edit));
                        break;
                    case "DeletePOI":
                        ConfirmDeletePOI(poiSelected);
                        break;
                }
            }
        }

        private void IconAdd_Click(object sender, EventArgs e)
        {
            ViewModel.EditPOICommand.Execute(new Tuple<int, Mode>(0, Mode.add));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            var appbar = this.Resources["AppBarList"] as ApplicationBar;
            if (appbar.Buttons != null)
            {
                (appbar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.Select;
                (appbar.Buttons[1] as ApplicationBarIconButton).Text = AppResources.AddPOI;
                (appbar.Buttons[2] as ApplicationBarIconButton).Text = AppResources.PlaceNearYou;
            }

            var appbarSelect = this.Resources["AppBarListSelect"] as ApplicationBar;
            if (appbarSelect.Buttons != null)
            {
                (appbarSelect.Buttons[0] as ApplicationBarIconButton).Text = AppResources.Delete;
            }
            ApplicationBar = this.Resources["AppBarList"] as ApplicationBar;
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = (POILLS.ItemsSource.Count > 0);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            locationError = false;

            if (e.IsNavigationInitiator)
                loadData();
            loadLocation();
        }

        private async void loadLocation()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50; // maybe 500 if error in this part...
            geolocator.MovementThreshold = 5;
            geolocator.ReportInterval = 500;

            try
            {
                ViewModel.Loading = true;
                ApplicationBar = this.Resources["AppBarList"] as ApplicationBar;
                (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = false;
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10));
                currentLatitude = geoposition.Coordinate.Point.Position.Latitude.ToString();
                currentLongitude = geoposition.Coordinate.Point.Position.Longitude.ToString();
                ViewModel.Loading = false;
                (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = true;

            }
            catch (Exception)
            {
                locationError = true;
                ViewModel.Loading = false;
                (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = true;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back && POILLS.IsSelectionEnabled)
            {
                POILLS.IsSelectionEnabled = false;
                e.Cancel = true;
            }
        }

        private void POILLS_IsSelectionEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (POILLS.IsSelectionEnabled)
                ApplicationBar = this.Resources["AppBarListSelect"] as ApplicationBar;
            else
                ApplicationBar = this.Resources["AppBarList"] as ApplicationBar;

            ApplicationBar.IsVisible = true;
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = !POILLS.IsSelectionEnabled;
        }

        private void POILLS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (POILLS.IsSelectionEnabled)
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = (POILLS.SelectedItems.Count > 0);
        }

        private void IconMultiSelect_Click(object sender, EventArgs e)
        {
            POILLS.IsSelectionEnabled = !POILLS.IsSelectionEnabled;
        }

        private void IconDelete_Click(object sender, EventArgs e)
        {
            ConfirmDeletePOI(null);
        }

        private void MyMap_Loaded(object sender, RoutedEventArgs e)
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
                MyMap.SetView(bounds);
            }
        }

        /// <summary>
        /// Message personnalisé pour savoir comment on gere les objets reliés au POI
        /// </summary>
        /// <param name="poiSelected"></param>
        private void ConfirmDeletePOI(PointOfInterest poiSelected)
        {
            CheckBox checkBoxDelete = new CheckBox()
            {
                Content = AppResources.DeletePOIObject,
                Margin = new Thickness(0, 14, 0, -2)
            };

            CheckBox checkBoxNull = new CheckBox()
            {
                Content = AppResources.NullPOIObject,
                Margin = new Thickness(0, 14, 0, -2)
            };

            //Simulation de grouped Checkbox
            checkBoxDelete.Checked += (s1, e1) =>
            {
                checkBoxNull.IsChecked = !(s1 as CheckBox).IsChecked;
            };
            checkBoxDelete.Unchecked += (s1, e1) =>
            {
                if (checkBoxNull.IsChecked == checkBoxDelete.IsChecked)
                    checkBoxDelete.IsChecked = !checkBoxDelete.IsChecked;
            };
            checkBoxNull.Unchecked += (s1, e1) =>
            {
                if (checkBoxNull.IsChecked == checkBoxDelete.IsChecked)
                    checkBoxNull.IsChecked = !checkBoxNull.IsChecked;
            };
            checkBoxNull.Checked += (s1, e1) =>
            {
                checkBoxDelete.IsChecked = !(s1 as CheckBox).IsChecked;
            };

            checkBoxNull.IsChecked = true;
            checkBoxDelete.IsChecked = false;

            TiltEffect.SetIsTiltEnabled(checkBoxDelete, true);
            TiltEffect.SetIsTiltEnabled(checkBoxNull, true);

            StackPanel stack = new StackPanel();
            stack.Orientation = System.Windows.Controls.Orientation.Vertical;
            stack.Children.Add(checkBoxDelete);
            stack.Children.Add(checkBoxNull);

            //Create a new custom message box
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                Caption = AppResources.Warning,
                Message = AppResources.POIObject,
                Content = stack,
                LeftButtonContent = "ok",
                RightButtonContent = AppResources.Cancel.ToLower(),
                IsFullScreen = false
            };

            //Define the dismissed event handler
            messageBox.Dismissed += (s1, e1) =>
            {
                ViewModel.DeletePOIObject = (bool)checkBoxDelete.IsChecked;

                if (e1.Result == CustomMessageBoxResult.LeftButton)
                {
                    if (poiSelected != null)
                    {
                        ViewModel.DeletePOICommand.Execute(poiSelected);
                        POILLS.ItemsSource = ViewModel.PointOfInterestList;
                    }
                    else
                    {
                        var placeToDeleteList = new List<object>(POILLS.SelectedItems as IList<object>);

                        ViewModel.DeletePOIsCommand.Execute(new List<object>(POILLS.SelectedItems as IList<object>));
                        POILLS.ItemsSource = ViewModel.PointOfInterestList;
                    }

                    (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = (POILLS.ItemsSource.Count > 0);

                }
            };

            //launch the task
            messageBox.Show();
        }

        private void startGeoNamesAPICall()
        {
            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)HttpWebRequest.Create(new Uri(AppResources.PlaceNearURI + "&lat=" + currentLatitude.Replace(",", ".") + "&lng=" + currentLongitude.Replace(",", ".") + "&username=" + AppResources.PlaceNearUsername + "&radius=5&maxRows=10"));
                httpReq.BeginGetResponse(HTTPWebRequestCallBack, httpReq);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void HTTPWebRequestCallBack(IAsyncResult result)
        {
            string strResponse = "";

            try
            {
                Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        HttpWebRequest httpRequest = (HttpWebRequest)result.AsyncState;
                        WebResponse response = httpRequest.EndGetResponse(result);
                        Stream stream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(stream);
                        strResponse = reader.ReadToEnd();

                        parseResponseData(strResponse);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void parseResponseData(String aResponse)
        {
            CheckMapp.Utils.PlaceNearToMap.PlacesList placesListObj = new CheckMapp.Utils.PlaceNearToMap.PlacesList();

            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(aResponse));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(placesListObj.GetType());
            placesListObj = ser.ReadObject(ms) as CheckMapp.Utils.PlaceNearToMap.PlacesList;
            ms.Close();

            // updating UI
            if (placesListObj != null)
            {
                updateMap(placesListObj);
            }
        }

        private void updateMap(CheckMapp.Utils.PlaceNearToMap.PlacesList aWiKIAPIResponse)
        {
            int totalRecords = aWiKIAPIResponse.PlaceList.Count();
            MyMap.Visibility = System.Windows.Visibility.Visible;

            try
            {
                ObservableCollection<CheckMapp.Utils.PlaceNearToMap.PlaceNearMap> placeToMapObjs = new ObservableCollection<CheckMapp.Utils.PlaceNearToMap.PlaceNearMap>();
                for (int index = 0; index < totalRecords; index++)
                {
                    placeToMapObjs.Add(new CheckMapp.Utils.PlaceNearToMap.PlaceNearMap()
                    {
                        Coordinate = new GeoCoordinate(Convert.ToDouble(aWiKIAPIResponse.PlaceList.ElementAt(index).Latitude, CultureInfo.InvariantCulture),
                                        Convert.ToDouble(aWiKIAPIResponse.PlaceList.ElementAt(index).Longitude, CultureInfo.InvariantCulture)),
                        Info = aWiKIAPIResponse.PlaceList.ElementAt(index).Title,
                        Summary = aWiKIAPIResponse.PlaceList.ElementAt(index).Summary

                    });
                }
                removeTempMapLayer();

                int countPOI = 0;
                foreach (CheckMapp.Utils.PlaceNearToMap.PlaceNearMap PlaceNear in placeToMapObjs)
                {
                    // If the set doesn't contains an element latitude+longitude we can show it trough pushPin nearby the phone location
                    if (!ViewModel.PointOfInterestList.Any(x => x.Latitude == PlaceNear.Coordinate.Latitude && x.Longitude == PlaceNear.Coordinate.Longitude))
                    {
                        MapLayer pinLayout = new MapLayer();
                        Pushpin MyPushpin = new Pushpin();
                        MapOverlay pinOverlay = new MapOverlay();

                        MyMap.Layers.Add(pinLayout);

                        MyPushpin.GeoCoordinate = PlaceNear.Coordinate;

                        pinOverlay.Content = MyPushpin;
                        pinOverlay.GeoCoordinate = PlaceNear.Coordinate;
                        pinOverlay.PositionOrigin = new Point(0, 1);
                        pinLayout.Add(pinOverlay);

                        MyPushpin.Content = PlaceNear.Info.Trim();

                        MyPushpin.Background = new SolidColorBrush(Color.FromArgb(255, 105, 105, 105));
                        MyPushpin.Tap += MyPushpin_Tap;
                        MyPushpin.Tag = PlaceNear;

                        countPOI++;
                    }
                }

                MessageBox.Show(String.Format(AppResources.ResultPOI, countPOI), AppResources.PlaceNearYou, MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured while updating map : " + ex.Message);
            }
        }

        void MyPushpin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            CheckMapp.Utils.PlaceNearToMap.PlaceNearMap placeNearYou = (CheckMapp.Utils.PlaceNearToMap.PlaceNearMap)(sender as Pushpin).Tag;

            //Create a new custom message box
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                Caption = placeNearYou.Info,
                Message = placeNearYou.Summary + Environment.NewLine + Environment.NewLine + AppResources.PlaceNearAdd,
                LeftButtonContent = AppResources.Add.ToLower(),
                RightButtonContent = AppResources.Cancel.ToLower(),
                IsFullScreen = false,
            };

            messageBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        PointOfInterest newPOI = new PointOfInterest();
                        newPOI.Trip = ViewModel.Trip;
                        newPOI.Latitude = placeNearYou.Coordinate.Latitude;
                        newPOI.Longitude = placeNearYou.Coordinate.Longitude;
                        newPOI.Name = placeNearYou.Info;
                        ViewModel.AddPOINearCommand.Execute(newPOI);
                        break;
                    default:
                        break;
                }
            };

            messageBox.Show();
        }

        private void IconNear_Click(object sender, EventArgs e)
        {
            if (Utility.checkNetworkConnection())
            {
                if (!locationError)
                {
                    if (isLocationFind())
                        startGeoNamesAPICall();
                }
                else
                    // the app does not have the right capability or the location master switch is off 
                    MessageBox.Show(AppResources.LocationError, AppResources.Warning, MessageBoxButton.OK);
            }
            else
                MessageBox.Show(AppResources.InternetConnectionSettings, AppResources.Warning, MessageBoxButton.OK);

        }

        private bool isLocationFind()
        {
            return (!string.IsNullOrEmpty(currentLatitude) && !string.IsNullOrEmpty(currentLongitude));
        }

        private void removeTempMapLayer()
        {
            while (MyMap.Layers.Count - 1 >= 1)
                MyMap.Layers.RemoveAt(MyMap.Layers.Count - 1);
        }



    }
}