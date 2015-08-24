using Microsoft.Phone.Maps.Toolkit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CheckMapp.Utils
{
    class PlaceNearToMap
    {
        public class PlaceNearMap
        {
            public GeoCoordinate Coordinate { get; set; }
            public string Summary { get; set; }
            public string Info { get; set; }
        }

        [DataContract]
        public class Place
        {
            [DataMember(Name = "summary")]
            public string Summary { get; set; }

            [DataMember(Name = "distance")]
            public string Distance { get; set; }

            [DataMember(Name = "rank")]
            public string Rank { get; set; }

            [DataMember(Name = "title")]
            public string Title { get; set; }

            [DataMember(Name = "wikipediaUrl")]
            public string WikipediaUrl { get; set; }

            [DataMember(Name = "elevation")]
            public string Elevation { get; set; }

            [DataMember(Name = "lng")]
            public string Longitude { get; set; }

            [DataMember(Name = "feature")]
            public string Feature { get; set; }

            [DataMember(Name = "lang")]
            public string Langage { get; set; }

            [DataMember(Name = "lat")]
            public string Latitude { get; set; }
        }

        [DataContract]
        public class PlacesList
        {
            [DataMember(Name = "geonames")]
            public List<Place> PlaceList { get; set; }
        }
    }
}
