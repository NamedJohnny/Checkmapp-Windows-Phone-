using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CheckMapp.Resources;
using CheckMapp.ViewModels.SettingsViewModels;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Live;
using Utility = CheckMapp.Utils.Utility;

namespace CheckMapp.Views.SettingsViews
{
    public partial class SettingsView : PhoneApplicationPage
    {
        CancellationTokenSource cts;
        public SettingsView()
        {
            InitializeComponent();
            PivotSettings.Header = AppResources.Settings.ToLower();

            //Seulement avec la version complète
            if (App.IsTrial)
            {
                btnImport.IsEnabled = false;
                btnExport.IsEnabled = false;
                btnImport.Visibility = System.Windows.Visibility.Visible;
                btnExport.Visibility = System.Windows.Visibility.Visible;
            }
        }

        public SettingsViewModel ViewModel
        {
            get
            {
                return this.DataContext as SettingsViewModel;
            }
        }

        #region Buttons


        private async void btnImport_Click(object sender, RoutedEventArgs e)
        {
            if (Utility.checkNetworkConnection() == false)
            {
                MessageBox.Show(AppResources.InternetConnectionSettings, AppResources.NotConnected, MessageBoxButton.OK);
                return;
            }
            if (MessageBox.Show(AppResources.ConfirmationImport, AppResources.Warning, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {

                ViewModel.ImportCommand.Execute(null);

                cts = new CancellationTokenSource();
                Progress<LiveOperationProgress> uploadProgress = new Progress<LiveOperationProgress>(
        (p) =>
        {
            ViewModel.ProgressPercent = p.ProgressPercentage;

        });
                int import = await Utility.ImportBD(cts.Token, uploadProgress);
                ViewModel.CancelCommand.Execute(null);
                if (import == 0)
                {
                    MessageBox.Show(AppResources.FileNotFound, AppResources.Warning, MessageBoxButton.OK);
                    return;
                }
                else if(import == 1)
                {
                    MessageBox.Show(AppResources.RestartApp, AppResources.Warning, MessageBoxButton.OK);
                    IsolatedStorageSettings.ApplicationSettings["ReplaceDB"] = true;
                    IsolatedStorageSettings.ApplicationSettings.Save();
                    Application.Current.Terminate();
                }
            }
        }

        private async void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (Utility.checkNetworkConnection() == false)
            {
                MessageBox.Show(AppResources.InternetConnectionSettings, AppResources.NotConnected, MessageBoxButton.OK);
                return;
            }

            ViewModel.ExportCommand.Execute(null);
            cts = new CancellationTokenSource();
            Progress<LiveOperationProgress> uploadProgress = new Progress<LiveOperationProgress>(
        (p) =>
        {
            ViewModel.ProgressPercent = p.ProgressPercentage;

        });
            int export = await Utility.ExportDB(cts.Token, uploadProgress);
            ViewModel.CancelCommand.Execute(null);
        }


        private void BtnRateApp_Click(object sender, EventArgs e)
        {
            this.Focus();
            MarketplaceDetailTask marketplaceDetailTask = new MarketplaceDetailTask();
            marketplaceDetailTask.ContentType = MarketplaceContentType.Applications;
            marketplaceDetailTask.ContentIdentifier = "f4c51a4a-35ef-4e72-a5c5-f9c8d0a4ebbd";
            marketplaceDetailTask.Show();
        }

        private void BtnWebsite_Click(object sender, EventArgs e)
        {
            // TODO: Change the URL with our real website
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri("https://www.checkmapp.com/");
            webBrowserTask.Show();
        }

        #endregion

        #region ToggleSwitch

        private void WifiOnlySwitch_Checked(object sender, RoutedEventArgs e)
        { ViewModel.WifiOnly = true; }

        private void WifiOnlySwitch_Unchecked(object sender, RoutedEventArgs e)
        { ViewModel.WifiOnly = false; }

        #endregion

       
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (cts != null)
                cts.Cancel();

            ViewModel.CancelCommand.Execute(null);
        }

      
    }
}