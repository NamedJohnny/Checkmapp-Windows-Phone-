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
using CheckMapp.Model.Tables;
using CheckMapp.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using CheckMapp.ViewModels.POIViewModels;

namespace CheckMapp.Views.NoteViews
{
    public partial class ListNoteView : PhoneApplicationPage
    {
        public ListNoteView()
        {
            InitializeComponent();
        }

        public ListNoteViewModel ViewModel
        {
            get
            {
                return this.DataContext as ListNoteViewModel;
            }
        }

        /// <summary>
        /// Ajout d'une note
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconAdd_Click(object sender, EventArgs e)
        {
            ViewModel.EditNoteCommand.Execute(new Tuple<int, Mode>(0, Mode.add));
        }

        /// <summary>
        /// On assigne les titres des boutons au démarrage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            var appbar = this.Resources["AppBarList"] as ApplicationBar;
            if (appbar.Buttons != null)
            {
                (appbar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.Select;
                (appbar.Buttons[1] as ApplicationBarIconButton).Text = AppResources.AddNote;
            }

            var appbarSelect = this.Resources["AppBarListSelect"] as ApplicationBar;
            if (appbarSelect.Buttons != null)
            {
                (appbarSelect.Buttons[0] as ApplicationBarIconButton).Text = AppResources.Delete;
            }

            ApplicationBar = this.Resources["AppBarList"] as ApplicationBar;
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = (NoteLLS.ItemsSource.Count > 0);
        }

        /// <summary>
        /// Click sur les options du menu contextuel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null && ((sender as MenuItem).DataContext is Note))
            {
                Note noteSelected = ((sender as MenuItem).DataContext as Note);
                switch (menuItem.Name)
                {
                    case "EditNote":
                        ViewModel.EditNoteCommand.Execute(new Tuple<int, Mode>(noteSelected.Id, Mode.edit));
                        break;
                    case "DeleteNote":
                        if (MessageBox.Show(AppResources.ConfirmationDeleteNote, "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            ViewModel.DeleteNoteCommand.Execute(noteSelected);
                            NoteLLS.ItemsSource = ViewModel.GroupedNotes;
                            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = (NoteLLS.ItemsSource.Count > 0);
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// J'ai besoin de ça pour mettre à jour mon ContextMenu lorsque je reviens à un changement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            var menu = (ContextMenu)sender;
            var owner = (FrameworkElement)menu.Owner;
            if (owner.DataContext != menu.DataContext)
                menu.DataContext = owner.DataContext;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NoteLLS.ItemsSource = (this.DataContext as ListNoteViewModel).GroupedNotes;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back && NoteLLS.IsSelectionEnabled)
            {
                NoteLLS.IsSelectionEnabled = false;
                e.Cancel = true;
            }
            if (e.Uri.OriginalString.Contains("ListPOI"))
            {
                Messenger.Default.Send<int, ListPOIViewModel>(ViewModel.Trip.Id);
            }
        }

        private void IconMultiSelect_Click(object sender, EventArgs e)
        {
            NoteLLS.IsSelectionEnabled = !NoteLLS.IsSelectionEnabled;
        }

        private void StackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NoteLLS.IsSelectionEnabled = false;

            if (!NoteLLS.IsSelectionEnabled)
            {
                Note itemTapped = (sender as FrameworkElement).DataContext as Note;
                ViewModel.NoteCommand.Execute(itemTapped.Id);
            }
        }

        private void IconDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(AppResources.ConfirmationDeleteNotes, "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {

                List<Note> noteList = new List<Note>();

                ViewModel.DeleteNotesCommand.Execute(new List<object>(NoteLLS.SelectedItems as IList<object>));
                NoteLLS.ItemsSource = ViewModel.GroupedNotes;

                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = (NoteLLS.ItemsSource.Count > 0);
            }
        }

        private void NoteLLS_IsSelectionEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (NoteLLS.IsSelectionEnabled)
                ApplicationBar = this.Resources["AppBarListSelect"] as ApplicationBar;
            else
                ApplicationBar = this.Resources["AppBarList"] as ApplicationBar;

            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = !NoteLLS.IsSelectionEnabled;
        }

        private void NoteLLS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NoteLLS.IsSelectionEnabled)
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = (NoteLLS.SelectedItems.Count > 0);
        }
    }
}