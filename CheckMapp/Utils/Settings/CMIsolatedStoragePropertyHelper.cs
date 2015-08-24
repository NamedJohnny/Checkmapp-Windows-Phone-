using System.IO.IsolatedStorage;

namespace CheckMapp.Utils.Settings
{
    /// <summary>
    /// Helper class is needed because IsolatedStorageProperty is generic and 
    /// can not provide singleton model for static content
    /// </summary>
    internal static class CMIsolatedStoragePropertyHelper
    {
        public static readonly object ThreadLocker = new object();
        public static readonly IsolatedStorageSettings Store = IsolatedStorageSettings.ApplicationSettings;
    }
}
