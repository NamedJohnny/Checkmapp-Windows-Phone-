using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CheckMapp.Model.Tables;
using CheckMapp.ViewModel;
using CheckMapp.ViewModels.ArchivesViewModels;
using System.Collections.ObjectModel;
using Microsoft.Practices.ServiceLocation;
using System.Windows.Data;
using CheckMapp.Controls;

namespace CheckMapp.Views.ArchivesViews
{
    public partial class TimelineView : UserControl
    {
        public TimelineView()
        {
            InitializeComponent();
            timelineControl.Trips = ViewModel.ArchiveTripList;
        }

        public TimelineViewModel ViewModel
        {
            get
            {
                return this.DataContext as TimelineViewModel;
            }
        }

        void timelineControl_UserControlElementTap(object sender, EventArgs e)
        {
            ViewModel.TripCommand.Execute((sender as Trip).Id);
        }

    }
}
