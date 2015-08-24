using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace CheckMapp.Controls
{
    public partial class LiveTilePicture : UserControl
    {
        public LiveTilePicture()
        {
            InitializeComponent();
            Storyboard anim = (Storyboard)FindName("liveTileAnimTop");
            anim.Begin();
        }

        private void liveTileAnimTop_Completed_1(object sender, EventArgs e)
        {
            Storyboard anim = (Storyboard)FindName("liveTileAnimBottom");
            anim.Begin();
        }

        private void liveTileAnimBottom_Completed_1(object sender, EventArgs e)
        {
            Storyboard anim = (Storyboard)FindName("liveTileAnimTop");
            anim.Begin();
        }

        public static readonly DependencyProperty SourceImageProperty =
           DependencyProperty.Register("SourceImage", typeof(BitmapImage), typeof(LiveTilePicture), null);

        /// <summary>
        /// La source de l'image
        /// </summary>
        public BitmapImage SourceImage
        {
            get { return base.GetValue(SourceImageProperty) as BitmapImage; }
            set
            {
                base.SetValue(SourceImageProperty, value);
            }
        }

        bool hasStarted = false;

        private void imgPhoto_Loaded(object sender, RoutedEventArgs e)
        {
            if (!hasStarted)
                splineDouble.Value = -imgPhoto.ActualHeight - 200;
            hasStarted = true;

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            imgPhoto.Source = SourceImage;
            if (SourceImage == null)
            {
                BitmapImage logo = new BitmapImage();
                logo.UriSource = new Uri(@"/Assets/Logo.png", UriKind.Relative);
                imgPhoto.Source = logo;
            }
        }
    }
}
