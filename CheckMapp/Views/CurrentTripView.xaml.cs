﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Animation;
using System.Windows.Media;
using CheckMapp.ViewModel;

namespace CheckMapp.Views
{
    public partial class CurrentTripView : PhoneApplicationPage
    {
        public CurrentTripView()
        {
            InitializeComponent();
            this.DataContext = new CurrentTripViewModel();
        }


    }
}