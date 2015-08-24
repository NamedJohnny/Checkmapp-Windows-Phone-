using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CheckMapp.ViewModels.POIViewModels;
using CheckMapp.Resources;
using CheckMapp.Model.Tables;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using CheckMapp.Utils;
using CheckMapp.ViewModels;

namespace CheckMapp.Views.POIViews
{
    public partial class AddEditPOIView : PhoneApplicationPage
    {
        public AddEditPOIView()
        {
            InitializeComponent();
        }

        public AddEditPOIViewModel ViewModel
        {
            get
            {
                return this.DataContext as AddEditPOIViewModel;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            //Assigne le titre de la page
            if (ViewModel.Mode == Mode.add || ViewModel.Mode == Mode.addFromExisting)
                TitleTextBox.Text = AppResources.AddPOI.ToLower();
            else if (ViewModel.Mode == Mode.edit)
                TitleTextBox.Text = AppResources.EditPoi.ToLower();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //On annule les changeemnts si l'usager fait BACK
            if (e.NavigationMode == NavigationMode.Back && !ViewModel.IsFormValid)
                ViewModel.CancelPOICommand.Execute(null);
        }

        /// <summary>
        /// On assigne les titres des boutons au démarrage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApplicationBar.Buttons != null)
            {
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.Save;
                (ApplicationBar.Buttons[1] as ApplicationBarIconButton).Text = AppResources.Cancel;
            }
        }

        /// <summary>
        /// Sauvegarde du POI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconSave_Click(object sender, EventArgs e)
        {
            this.Focus();
            // wait till the next UI thread tick so that the binding gets updated
            Dispatcher.BeginInvoke(() =>
            {
                ViewModel.AddPOICommand.Execute(null);

                if (ViewModel.IsFormValid)
                    ViewModel.GoBackCommand.Execute(null);
            });

        }

        /// <summary>
        /// Annuler l'ajout du POI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconCancel_Click(object sender, EventArgs e)
        {
            ViewModel.GoBackCommand.Execute(null);
        }

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ViewModel.SelectTypeCommand.Execute(null);
        }

    }
}