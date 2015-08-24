using CheckMapp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;

namespace CheckMapp
{
    /// <summary>
    /// Provides access to string resources.
    /// </summary>
    public class LocalizedStrings
    {
        private static AppResources _localizedResources = new AppResources();

        public AppResources LocalizedResources { get { return _localizedResources; } }

        /// We need to provide ability to change app language at runtime
        /// So we need to notify views which had binded string resource about change 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Call this method using LocalizedStrings.LocalizedStringsResource.UpdateLanguage()
        /// When you had switched current ui language for the application and want to notify your views about that 
        /// </summary>
        public void UpdateLanguage()
        {
            // To prevent NPE, if another thread modify event handler after check
            var handler = PropertyChanged;
            if (handler != null) handler(LocalizedResources, new PropertyChangedEventArgs(null));
        }

        public static LocalizedStrings LocalizedStringsResource
        {
            get
            {
                return Application.Current.Resources["LocalizedStrings"] as LocalizedStrings;
            }
        }
    }
}