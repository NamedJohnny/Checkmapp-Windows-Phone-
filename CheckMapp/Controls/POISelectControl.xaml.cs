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
using System.Collections.ObjectModel;

namespace CheckMapp.Controls
{
    public partial class POISelectControl : UserControl
    {
        public POISelectControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PoiListProperty =
          DependencyProperty.Register("PoiList", typeof(List<PointOfInterest>), typeof(POISelectControl), null);

        public List<PointOfInterest> PoiList 
        {
            get { return GetValue(PoiListProperty) as List<PointOfInterest>; }
            set
            {
                SetValue(PoiListProperty, value);
            }
        }

        public static readonly DependencyProperty POISelectedProperty =
        DependencyProperty.Register("POISelected", typeof(PointOfInterest), typeof(POISelectControl), null);

        public PointOfInterest POISelected
        {
            get { return GetValue(POISelectedProperty) as PointOfInterest; }
            set
            {
                SetValue(POISelectedProperty, value);
            }
        }

        public static readonly DependencyProperty NoneCheckProperty =
       DependencyProperty.Register("NoneCheck", typeof(bool), typeof(POISelectControl), null);

        public bool NoneCheck
        {
            get { return (bool)GetValue(NoneCheckProperty); }
            set
            {
                SetValue(NoneCheckProperty, value);
            }
        }


        private void chkNoPOI_Checked(object sender, RoutedEventArgs e)
        {
            chkHide_Storyboard.Begin();
            poiListPicker.Visibility = Visibility.Collapsed;
        }

        private void chkNoPOI_UnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                chkShow_Storyboard.Begin();
                poiListPicker.Visibility = Visibility.Visible;
                poiListPicker.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                chkNoPOI.IsEnabled = false;
                chkNoPOI.IsChecked = true;
                Console.WriteLine("Exception occured while unchecking poi, Disabling chkNoPoi : " + ex.Message);
            }
        }

        public void CheckboxState()
        {
            chkNoPOI.IsEnabled = (PoiList != null && PoiList.Count > 0);
            chkNoPOI.IsChecked = (PoiList == null || PoiList.Count == 0 || POISelected == null);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CheckboxState();
        }

    }
}
