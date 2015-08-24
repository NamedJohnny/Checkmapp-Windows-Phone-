using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using System.Xml;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using System.Windows;
using System.Device.Location;
using Windows.Storage;
using System.Runtime.Serialization;
using CheckMapp.Resources;
using System.IO.IsolatedStorage;
using CheckMapp.Model;
using Microsoft.Phone.Data.Linq;
using CheckMapp.ViewModels;
using Microsoft.Phone.Shell;
using CheckMapp.Model.DataService;
using CheckMapp.Model.Tables;
using System.Threading;
using CheckMapp.Utils.Settings;
using Microsoft.Live;
using Microsoft.Phone.BackgroundTransfer;

namespace CheckMapp.Utils
{
    public static class Utility
    {

        #region Images Functions

        public static BitmapImage ByteArrayToImage(byte[] imageByteArray, bool compress)
        {
            BitmapImage img = new BitmapImage();
            if (compress)
            {
                img.DecodePixelHeight = 100;
                img.DecodePixelType = DecodePixelType.Logical;
                img.CreateOptions = BitmapCreateOptions.DelayCreation;
            }
            using (MemoryStream memStream = new MemoryStream(imageByteArray))
            {
                try
                {
                    img.SetSource(memStream);
                }
                catch(Exception e)
                {
                    return new BitmapImage();
                }
            }

            return img;
        }

        public static byte[] ConvertToBytes(BitmapImage bitmapImage)
        {
            byte[] data = null;
            using (MemoryStream stream = new MemoryStream())
            {
                WriteableBitmap wBitmap = new WriteableBitmap(bitmapImage);
                wBitmap.SaveJpeg(stream, wBitmap.PixelWidth, wBitmap.PixelHeight, 0, 40);
                stream.Seek(0, SeekOrigin.Begin);
                data = stream.GetBuffer();
            }
            return data;
        }

        #endregion

        #region Stream Functions

        /// <summary>
        /// Transforme le stream d'une photo en tableau de byte
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] ReadFully(Stream input)
        {
            if (input == null)
                return null;

            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        #endregion

        #region Internet Connection Functions

        /// <summary>
        /// Vérifie qu'il y est une connexion active
        /// Vérifie en même temps si on doit uniquement se servir du Wifi
        /// </summary>
        /// <param name=></param>
        /// <returns>true si connecté à un réseaux</returns>
        public static bool checkNetworkConnection()
        {
            var ni = NetworkInterface.NetworkInterfaceType;
            bool IsConnected = false;

            if (GetWifiStorageProperty())
                IsConnected = IsConnectedOnWifi();
            else if ((ni == NetworkInterfaceType.Wireless80211) ||
                (ni == NetworkInterfaceType.MobileBroadbandCdma) ||
                (ni == NetworkInterfaceType.MobileBroadbandGsm))
                IsConnected = true;

            return IsConnected;
        }

        #endregion

        #region Map
        public static async Task AddLocation(Microsoft.Phone.Maps.Controls.Map myMap, Microsoft.Phone.Controls.PhoneTextBox myTextBox, System.Windows.Input.GestureEventArgs e, double latitude, double longitude, bool completeAddress = false, string pinContent = "")
        {
            ReverseGeocodeQuery query;
            List<MapLocation> mapLocations;
            string pushpinContent = "";
            MapLocation mapLocation;

            query = new ReverseGeocodeQuery();
            if (e != null)
                query.GeoCoordinate = myMap.ConvertViewportPointToGeoCoordinate(e.GetPosition(myMap));
            else
                query.GeoCoordinate = new GeoCoordinate(latitude, longitude);

            mapLocations = (List<MapLocation>)await query.GetMapLocationsAsync();
            mapLocation = mapLocations.FirstOrDefault();

            if (mapLocation != null && !String.IsNullOrEmpty(mapLocation.Information.Address.Country))
            {
                MapLayer pinLayout = new MapLayer();
                Pushpin MyPushpin = new Pushpin();
                MapOverlay pinOverlay = new MapOverlay();
                if (myMap.Layers.Count > 0)
                {
                    myMap.Layers.RemoveAt(myMap.Layers.Count - 1);
                }

                myMap.Layers.Add(pinLayout);

                MyPushpin.GeoCoordinate = mapLocation.GeoCoordinate;


                pinOverlay.Content = MyPushpin;
                pinOverlay.GeoCoordinate = mapLocation.GeoCoordinate;
                pinOverlay.PositionOrigin = new Point(0, 1);
                pinLayout.Add(pinOverlay);

                if (!completeAddress)
                    pushpinContent = getAddress(mapLocation);
                else
                    pushpinContent = getCompleteAddress(mapLocation);

                if (!string.IsNullOrEmpty(pinContent))
                    pushpinContent = pinContent;

                MyPushpin.Content = pushpinContent.Trim();
                if (myTextBox != null)
                    myTextBox.Text = MyPushpin.Content.ToString();
            }
        }

        private static string getAddress(MapLocation mapLocation)
        {
            string Address = "";
            string region = MapHelper.getRegion(mapLocation);
            string city = mapLocation.Information.Address.City;
            string country = mapLocation.Information.Address.Country;

            if (string.IsNullOrEmpty(region) && string.IsNullOrEmpty(city) && string.IsNullOrEmpty(country))
                Address = "";
            else if (string.IsNullOrEmpty(region) && string.IsNullOrEmpty(city))
                Address = country;
            else if (string.IsNullOrEmpty(region) && string.IsNullOrEmpty(country))
                Address = city;
            else if (string.IsNullOrEmpty(city) && string.IsNullOrEmpty(country))
                Address = region;
            else if (string.IsNullOrEmpty(region))
                Address = string.Format("{0}, {1} ", city, country);
            else if (string.IsNullOrEmpty(city))
                Address = string.Format("{0}, {1} ", region, country);
            else if (string.IsNullOrEmpty(country))
                Address = string.Format("{0}, {1} ", city, region);
            else
                Address = string.Format("{0}, {1}, {2} ", city, region, country);

            return Address;
        }

        private static string getCompleteAddress(MapLocation mapLocation)
        {
            string houseNumber = mapLocation.Information.Address.HouseNumber;
            string street = mapLocation.Information.Address.Street;
            string Address = "";
            string region = MapHelper.getRegion(mapLocation);
            string city = mapLocation.Information.Address.City;
            string country = mapLocation.Information.Address.Country;

            if (string.IsNullOrEmpty(region) && string.IsNullOrEmpty(city) && string.IsNullOrEmpty(country))
                Address = "";
            else if (string.IsNullOrEmpty(region) && string.IsNullOrEmpty(city))
                Address = country;
            else if (string.IsNullOrEmpty(region) && string.IsNullOrEmpty(country))
                Address = city;
            else if (string.IsNullOrEmpty(city) && string.IsNullOrEmpty(country))
                Address = region;
            else if (string.IsNullOrEmpty(region))
                Address = string.Format("{0}, {1} ", city, country);
            else if (string.IsNullOrEmpty(city))
                Address = string.Format("{0}, {1} ", region, country);
            else if (string.IsNullOrEmpty(country))
                Address = string.Format("{0}, {1} ", city, region);
            else if (string.IsNullOrEmpty(houseNumber) && string.IsNullOrEmpty(street))
                Address = string.Format("{0}, {1}, {2} ", city, region, country);
            else if (string.IsNullOrEmpty(houseNumber))
                Address = string.Format("{0}, {1}, {2}, {3}", street, city, region, country);
            else if (string.IsNullOrEmpty(street))
                Address = string.Format("{0}, {1}, {2}, {3}", houseNumber, city, region, country);
            else
                Address = string.Format("{0}, {1}, {2}, {3}, {4}", houseNumber, street, city, region, country);

            return Address;
        }

        #endregion

        #region XML

        public static string GetAppVersion()
        {
            var xmlReaderSettings = new XmlReaderSettings
            {
                XmlResolver = new XmlXapResolver()
            };

            using (var xmlReader = XmlReader.Create("WMAppManifest.xml", xmlReaderSettings))
            {
                xmlReader.ReadToDescendant("App");

                return xmlReader.GetAttribute("Version");
            }
        }

        #endregion

        #region Friends List

        public static List<string> FriendToList(string friends)
        {
            if (String.IsNullOrEmpty(friends))
            {
                return new List<string>();
            }
            return friends.Split(',').Where(x => !String.IsNullOrEmpty(x)).ToList();
        }

        public static string FriendToString(List<string> friends)
        {
            if (friends == null)
                return string.Empty;

            string friendString = null;
            foreach (string friend in friends)
            {
                friendString += friend + ",";
            }

            return friendString;
        }

        #endregion friend list

        #region OneDrive
        private static LiveConnectClient _liveClient = null;
        public static LiveConnectClient LiveClient
        {
            get
            {
                return _liveClient;
            }
            set
            {
                _liveClient = value;
            }
        }

        private async static Task<int> LogClient()
        {
            //  create OneDrive auth client
            var authClient = new LiveAuthClient("000000004814E746");

            //  ask for both read and write access to the OneDrive
            LiveLoginResult result = await authClient.LoginAsync(new string[] { "wl.skydrive", "wl.skydrive_update" });

            //  if login successful 
            if (result.Status == LiveConnectSessionStatus.Connected)
            {
                //  create a OneDrive client
                LiveClient = new LiveConnectClient(result.Session);
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// Uploader notre base de données sur OneDrive
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async static Task<int> ExportDB(CancellationToken ct, Progress<LiveOperationProgress> uploadProgress)
        {
            int log = 0;
            if (LiveClient == null)
                log = await LogClient();

            // Prepare for download, make sure there are no previous requests
            var reqList = BackgroundTransferService.Requests.ToList();
            foreach (var req in reqList)
            {
                if (req.UploadLocation.Equals(new Uri(@"\shared\transfers\" + AppResources.DBFileName, UriKind.Relative))
                    || req.UploadLocation.Equals(new Uri(@"\shared\transfers\" + AppResources.DBFileName + ".json", UriKind.Relative)))
                {
                    BackgroundTransferService.Remove(BackgroundTransferService.Find(req.RequestId));
                }
            }

            IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();
            iso.CopyFile(AppResources.DBFileName, "/shared/transfers/" + AppResources.DBFileName, true);

            //  create a folder
            string folderID = await GetFolderID("checkmapp");

            if (string.IsNullOrEmpty(folderID))
            {
                //  return error
                return 0;
            }

            //  upload local file to OneDrive
            LiveClient.BackgroundTransferPreferences = BackgroundTransferPreferences.AllowCellularAndBattery;
            try
            {
                await LiveClient.BackgroundUploadAsync(folderID, new Uri("/shared/transfers/" + AppResources.DBFileName, UriKind.RelativeOrAbsolute), OverwriteOption.Overwrite, ct, uploadProgress);
            }
            catch (TaskCanceledException exception)
            {
                Console.WriteLine("Exception occured while uploading file to OneDrive : " + exception.Message);
            }
            catch (Exception e)
            {

            }
            return 1;
        }

        /// <summary>
        /// Downloader la BD a partir de OneDrive
        /// </summary>
        /// <returns></returns>
        public async static Task<int> ImportBD(CancellationToken ct, Progress<LiveOperationProgress> uploadProgress)
        {
            int log = 0;
            if (LiveClient == null)
                log = await LogClient();

            // Prepare for download, make sure there are no previous requests
            var reqList = BackgroundTransferService.Requests.ToList();
            foreach (var req in reqList)
            {
                if (req.DownloadLocation.Equals(new Uri(@"\shared\transfers\" + AppResources.DBFileName, UriKind.Relative))
                    || req.DownloadLocation.Equals(new Uri(@"\shared\transfers\" + AppResources.DBFileName + ".json", UriKind.Relative)))
                {
                    BackgroundTransferService.Remove(BackgroundTransferService.Find(req.RequestId));
                }
            }

            string fileID = string.Empty;

            //  get folder ID
            string folderID = await GetFolderID("checkmapp");

            if (string.IsNullOrEmpty(folderID))
            {
                return 0; // doesnt exists
            }

            //  get list of files in this folder
            LiveOperationResult loResults = await LiveClient.GetAsync(folderID + "/files");
            List<object> folder = loResults.Result["data"] as List<object>;

            //  search for our file 
            foreach (object fileDetails in folder)
            {
                IDictionary<string, object> file = fileDetails as IDictionary<string, object>;
                if (string.Compare(file["name"].ToString(), AppResources.DBFileName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    //  found our file
                    fileID = file["id"].ToString();
                    break;
                }
            }

            if (string.IsNullOrEmpty(fileID))
            {
                //  file doesnt exists
                return 0;
            }

            try
            {
                //  download file from OneDrive
                LiveClient.BackgroundTransferPreferences = BackgroundTransferPreferences.AllowCellularAndBattery;
                await LiveClient.BackgroundDownloadAsync(fileID + @"/content", new Uri(@"\shared\transfers\" + AppResources.DBFileName, UriKind.RelativeOrAbsolute), ct, uploadProgress);
            }
            catch (TaskCanceledException exception)
            {
                Console.WriteLine("Exception occured while downloading file from OneDrive : " + exception.Message);
                return 2;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured while downloading file from OneDrive : " + ex.Message);
                return 0;
            }
            return 1;
        }

        /// <summary>
        /// Obtient le dossier dans Onedrive
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public async static Task<string> GetFolderID(string folderName)
        {
            try
            {
                string queryString = "me/skydrive/files?filter=folders";
                //  get all folders
                LiveOperationResult loResults = await LiveClient.GetAsync(queryString);
                dynamic folders = loResults.Result;

                foreach (dynamic folder in folders.data)
                {
                    if (string.Compare(folder.name, folderName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        //  found our folder
                        return folder.id;
                    }
                }

                //  folder not found

                //  create folder
                Dictionary<string, object> folderDetails = new Dictionary<string, object>();
                folderDetails.Add("name", folderName);
                loResults = await LiveClient.PostAsync("me/skydrive", folderDetails);
                folders = loResults.Result;

                // return folder id
                return folders.id;
            }
            catch
            {
                return string.Empty;
            }
        }


        #endregion

        #region POIType

        public static string ImageSourceFromPOIType(POIType type)
        {
            return "/Images/POIType/" + type.ToString().ToLower() + ".png";
        }

        public static string Display(this POIType type)
        {
            return AppResources.ResourceManager.GetString(type.ToString(), AppResources.Culture);
        }

        #endregion

        #region Settings

        /// <summary>
        /// Check if we have to use only the wifi
        /// </summary>
        /// <returns></returns>
        public static bool GetWifiStorageProperty()
        {
            return CMSettingsContainer.WifiOnly.Value;
        }

        /// <summary>
        /// Check if a wifi connection is already established
        /// </summary>
        /// <returns></returns>
        public static bool IsConnectedOnWifi()
        {
            var ni = NetworkInterface.NetworkInterfaceType;

            if (ni == NetworkInterfaceType.Wireless80211)
                return true;

            return false;
        }

        #endregion
    }
}
