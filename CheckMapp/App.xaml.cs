using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CheckMapp.Resources;
using CheckMapp.ViewModel;
using System.Globalization;
using System.Threading;
using CheckMapp.Model;
using CheckMapp.Model.Tables;
using System.Linq;
using CheckMapp.Model.DataService;
using System.IO.IsolatedStorage;
using System.IO;
using CheckMapp.Utils.Languages;
using System.Windows.Controls.Primitives;
using CheckMapp.Controls;

namespace CheckMapp
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        // Specify the local database connection string.
        public static string DBConnectionString = "Data Source=isostore:/" + AppResources.DBFileName;

        public static DatabaseDataContext db = new DatabaseDataContext(App.DBConnectionString);

        private static TimeSpan TrialPeriodLength = TimeSpan.FromDays(30);
        private const string FirstLauchDateKey = "FirstLaunchDate";
        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;


            if (IsolatedStorageSettings.ApplicationSettings.Contains("ReplaceDB") &&
                (bool)IsolatedStorageSettings.ApplicationSettings["ReplaceDB"] == true)
            {
                // Obtain the virtual store for the application.
                IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();
                //Replace database
                if (iso.FileExists(AppResources.DBFileName))
                    iso.DeleteFile(AppResources.DBFileName);
                if (iso.FileExists("/shared/transfers/" + AppResources.DBFileName))
                    iso.MoveFile("/shared/transfers/" + AppResources.DBFileName, AppResources.DBFileName);
                IsolatedStorageSettings.ApplicationSettings["ReplaceDB"] = false;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
            else
            {
                // Create the database if it does not exist.
                using (DatabaseDataContext db = new DatabaseDataContext(DBConnectionString))
                {
                    if (db.DatabaseExists() == false)
                    {
                        // Create the local database.
                        db.CreateDatabase();
                        db.SubmitChanges();
                    }
                }
            }

            // Standard XAML initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();
            
            // Set the correct language when application is launched
            InitializeLanguage();


            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                // Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }


            //Si c'est un redemarrage suite a l'importation
            
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            this.CheckTrialState();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
           
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {

        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            ViewModelLocator.Cleanup();

        }

        // Use static property for caching and easy access to the trial state
        public static bool IsTrial
        {
            get;
            // setting the IsTrial property from outside is not allowed
            private set;
        }

        public static bool IsTrialOver
        {
            get;
            private set;
        }

        public static int DaysRemaining
        {
            get;
            private set;
        }

        private void DetermineIsTrial()
        {
#if TRIAL
    // return true if debugging with trial enabled (DebugTrial configuration is active)
    IsTrial = true;
#else
            var license = new Microsoft.Phone.Marketplace.LicenseInformation();
            IsTrial = license.IsTrial();
#endif
        }

        private void CheckTrialState()
        {
            // refresh the value of the IsTrial property 
            this.DetermineIsTrial();

            if (!IsTrial)
            {
                // do not execute further if app is full version
                return;
            }

            this.CheckTrialPeriodExpired();
        }

        private void CheckTrialPeriodExpired()
        {
            // when the application is activated
            // show message to buy the full version if trial period has expired
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            DateTime firstLauchDate;
            if (settings.TryGetValue<DateTime>(FirstLauchDateKey, out firstLauchDate))
            {
                TimeSpan timeSinceFirstLauch = DateTime.UtcNow.Subtract(firstLauchDate);
                if (timeSinceFirstLauch > TrialPeriodLength)
                    IsTrialOver = true;
                else
                {
                    TimeSpan elapsed = TrialPeriodLength.Subtract(timeSinceFirstLauch);
                    if(elapsed!=null)
                        DaysRemaining = (int)elapsed.TotalDays + 1;
                    IsTrialOver = false;
                }

                // subscribe to the Navigated event in order to show the popup
                // over the page after it has loaded
                RootFrame.Navigated += new NavigatedEventHandler(RootFrame_Navigated);
            }
            else
            {
                // if a value cannot be found for the first launch date
                // save the current date and time 
                settings.Add(FirstLauchDateKey, DateTime.UtcNow);
                settings.Save();
            }
        }

        void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // remove the Navigated event handler as it is no longer necessary
            RootFrame.Navigated -= new NavigatedEventHandler(RootFrame_Navigated);

            Popup popup = new Popup();
            BuyNowUserControl content = new BuyNowUserControl(popup);
            content.DaysRemaining = DaysRemaining;
            content.IsTrialFinish = IsTrialOver;
            // set the width of the popup to the width of the screen
            content.Width = System.Windows.Application.Current.Host.Content.ActualWidth;
            popup.Child = content;
            popup.VerticalOffset = 300;
            popup.IsOpen = true;
            popup.Closed += popup_Closed;
        }

        void popup_Closed(object sender, EventArgs e)
        {
            if (IsTrialOver)
                Application.Current.Terminate();
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            string lang = LocalizationManager.GetCurrentAppLang();
            if (LocalizationManager.GetCurrentAppLang() == "en")
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            else if (LocalizationManager.GetCurrentAppLang() == "fr")
                Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");

            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            DispatcherHelper.Initialize();

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion

        // Initialize the app's font and flow direction as defined in its localized resource strings.
        //
        // To ensure that the font of your application is aligned with its supported languages and that the
        // FlowDirection for each of those languages follows its traditional direction, ResourceLanguage
        // and ResourceFlowDirection should be initialized in each resx file to match these values with that
        // file's culture. For example:
        //
        // AppResources.es-ES.resx
        //    ResourceLanguage's value should be "es-ES"
        //    ResourceFlowDirection's value should be "LeftToRight"
        //
        // AppResources.ar-SA.resx
        //     ResourceLanguage's value should be "ar-SA"
        //     ResourceFlowDirection's value should be "RightToLeft"
        //
        // For more info on localizing Windows Phone apps see http://go.microsoft.com/fwlink/?LinkId=262072.
        //
        private void InitializeLanguage()
        {
            try
            {
                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the Display
                // language of the phone is not supported.
                //
                // If a compiler error is hit then ResourceLanguage is missing from
                // the resource file.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                // If a compiler error is hit then ResourceFlowDirection is missing from
                // the resource file.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;

                //Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }
    }
}