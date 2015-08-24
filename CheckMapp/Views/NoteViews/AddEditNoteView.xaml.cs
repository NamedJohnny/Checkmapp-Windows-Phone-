using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CheckMapp.ViewModels.NoteViewModels;
using CheckMapp.Resources;
using CheckMapp.ViewModels;
using CheckMapp.Model.Tables;
using Utility = CheckMapp.Utils.Utility;

namespace CheckMapp.Views.NoteViews
{
    public partial class AddEditNoteView : PhoneApplicationPage
    {
        public AddEditNoteView()
        {
            InitializeComponent();
            LoadPage();
        }

        public AddEditNoteViewModel ViewModel
        {
            get
            {
                return this.DataContext as AddEditNoteViewModel;
            }
        }

        /// <summary>
        /// Sauvegarder la note
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconSave_Click(object sender, EventArgs e)
        {
            this.Focus();

            // wait till the next UI thread tick so that the binding gets updated
            Dispatcher.BeginInvoke(() =>
            {
                ViewModel.AddEditNoteCommand.Execute(null);

                if (ViewModel.IsFormValid)
                {
                    ViewModel.GoBackCommand.Execute(null);
                }
            });
        }

        /// <summary>
        /// Annuler l'ajout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconCancel_Click(object sender, EventArgs e)
        {
            ViewModel.GoBackCommand.Execute(null);
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
            if (ViewModel.POISelected != null)
                POIControl.chkNoPOI.IsChecked = false;
        }

        private void LoadPage()
        {
            //Assigne le titre de la page
            if (ViewModel.Mode == Mode.add)
                TitleTextblock.Text = AppResources.AddNote.ToLower();
            else if (ViewModel.Mode == Mode.edit)
                TitleTextblock.Text = AppResources.EditNote.ToLower();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //On annule les changeemnts si l'usager fait BACK
            if (e.NavigationMode == NavigationMode.Back && !ViewModel.IsFormValid)
                ViewModel.CancelNoteCommand.Execute(null);

        }

    }
}