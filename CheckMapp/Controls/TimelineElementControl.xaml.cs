using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using CheckMapp.Model.Tables;

namespace CheckMapp.Controls
{
    public partial class TimelineElementControl : UserControl
    {


        public TimelineElementControl(Brush color, bool left)
        {
            this.DataContext = this;
            
            InitializeComponent();
            if (left)
            {
                this.left.Visibility = System.Windows.Visibility.Visible;
                this.right.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.right.Visibility = System.Windows.Visibility.Visible;
                this.left.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public static readonly DependencyProperty TripProperty =
           DependencyProperty.Register("Trips", typeof(Trip), typeof(TimelineElementControl), null);

        /// <summary>
        /// Un évenement de la ligne
        /// </summary>
        public Trip Trip
        {
            get { return GetValue(TripProperty) as Trip; }
            set { 
                SetValue(TripProperty, value); }
        }


    }
}
