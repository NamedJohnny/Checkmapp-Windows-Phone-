using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using CheckMapp.Utils.Settings;

namespace CheckMapp.Utils.Languages
{
    public static class LocalizationManager
    {
        private static event EventHandler<LocalizationChangedEventArgs> LocalizationChanged;
        private static readonly object LocalizationManagerLock = new object();
        private static CMIsolatedStorageProperty<string> LanguageStorageProperty;

        static LocalizationManager()
        {
            LanguageStorageProperty = CMSettingsContainer.Language;
        }

        public static string GetCurrentAppLang()
        {
            lock (LocalizationManagerLock)
            {
                string savedLang = LanguageStorageProperty.Value;
                if (savedLang != null) return savedLang;

                var currentCulture = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

                // returning language from current UI culture only if it is supported by our app, otherwise reutrning EN as default
                foreach (var lang in SupportedLanguages.langDictionary.Keys.Where(lang => lang == currentCulture)) return lang;

                return SupportedLanguages.En;
            }
        }
       
        /// <summary>
        /// Get all languages 2 caracters code
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllLanguagesCode()
        {
            return SupportedLanguages.langDictionary.Keys.ToList();
        }

        /// <summary>
        /// Get all languages
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllLanguages()
        {
            return SupportedLanguages.langDictionary.Values.ToList();
        }

        /// <summary>
        /// Changes current app language, updates all views and saves current language
        /// if language is not supported, it will not switch to it
        /// </summary>
        /// <param name="lang"></param>
        public static void ChangeAppLanguage(string lang)
        {
            lock (LocalizationManagerLock)
            {
                // Can not switch to not supported language
                if (!GetAllLanguagesCode().Contains(lang)) return;

                Thread.CurrentThread.CurrentUICulture = new CultureInfo(GetAllLanguagesCode().First(x=>x.Contains(lang)));
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;
                LocalizedStrings.LocalizedStringsResource.UpdateLanguage();

                // saving new language
                LanguageStorageProperty.Value = lang;

                // notifying binded views about language change, so they will reload resources automatically
                NotifyLocalizationChanged(lang);
            }
        }

        /// <summary>
        /// Resets current App language to the system language
        /// </summary>
        public static void ResetAppLanguageToTheSystemLanguage()
        {
            ChangeAppLanguage(Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName);
        }

        private static void NotifyLocalizationChanged(string newLocalization)
        {
            var localizationChanged = LocalizationChanged;

            if (localizationChanged != null)
            {
                LocalizationChanged(null, new LocalizationChangedEventArgs(newLocalization));
            }
        }
    }
}
