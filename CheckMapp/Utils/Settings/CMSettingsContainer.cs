using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckMapp.Utils.Settings
{
    public static class CMSettingsContainer
    {
        public static readonly CMIsolatedStorageProperty<bool> WifiOnly
            = new CMIsolatedStorageProperty<bool>("WifiOnly");

        public static readonly CMIsolatedStorageProperty<string> Language
            = new CMIsolatedStorageProperty<string>("Language");
    }
}
