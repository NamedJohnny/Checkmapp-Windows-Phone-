using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Windows.Controls.Primitives;
using CheckMapp.Resources;

namespace CheckMapp.Controls
{
    public partial class BuyNowUserControl : UserControl
    {
        private Popup popup = null;

        public BuyNowUserControl()
            : this(null)
        {
        }

        public BuyNowUserControl(Popup popup)
        {
            InitializeComponent();
            this.popup = popup;
            this.DataContext = this;
        }

        private void btnBuyNow_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceDetailTask marketplaceDetailTask = new MarketplaceDetailTask();
            marketplaceDetailTask.ContentType = MarketplaceContentType.Applications;
            marketplaceDetailTask.ContentIdentifier = "f4c51a4a-35ef-4e72-a5c5-f9c8d0a4ebbd";
            marketplaceDetailTask.Show();
            this.ClosePopup();
        }

        private void btnContinueTrial_Click(object sender, RoutedEventArgs e)
        {
            this.ClosePopup();
        }

        public int DaysRemaining
        {
            get;
            set;
        }

        public static readonly DependencyProperty IsTrialFinishProperty =
           DependencyProperty.Register("IsTrialFinish", typeof(bool), typeof(BuyNowUserControl), null);

        /// <summary>
        /// La source de l'image
        /// </summary>
        public bool IsTrialFinish
        {
            get { return (bool)base.GetValue(IsTrialFinishProperty); }
            set
            {
                base.SetValue(IsTrialFinishProperty, value);

                if (value)
                {
                    titleTextBox.Text = AppResources.TrialOver;
                }
                else
                {
                    titleTextBox.Text = String.Format(AppResources.BuyApp, DaysRemaining);
                }
            }
        }

        private void ClosePopup()
        {
            if (this.popup != null)
            {
                this.popup.IsOpen = false;
            }
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            ClosePopup();
        }
    }
}
