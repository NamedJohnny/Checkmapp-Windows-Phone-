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
using Utility = CheckMapp.Utils.Utility;
using Microsoft.Phone.Tasks;

namespace CheckMapp.Views.NoteViews
{
    public partial class NoteView : PhoneApplicationPage
    {
        public NoteView()
        {
            InitializeComponent();
        }

        public NoteViewModel ViewModel
        {
            get
            {
                return this.DataContext as NoteViewModel;
            }
        }
        private void IconDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(AppResources.ConfirmationDeleteNote, "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                // Call the appropriate function in ViewModel
                var vm = DataContext as NoteViewModel;
                vm.DeleteNoteCommand.Execute(null);
            }
        }

        private void IconEdit_Click(object sender, EventArgs e)
        {
            ViewModel.EditNoteCommand.Execute(Mode.edit);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApplicationBar.Buttons != null && ApplicationBar.MenuItems != null)
            {
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Text = AppResources.Previous;
                (ApplicationBar.Buttons[1] as ApplicationBarIconButton).Text = AppResources.Next;

                (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = AppResources.Share;
                (ApplicationBar.MenuItems[1] as ApplicationBarMenuItem).Text = AppResources.Edit;
                (ApplicationBar.MenuItems[2] as ApplicationBarMenuItem).Text = AppResources.Delete;
            }
            EnableArrow();
        }

        private void IconShare_Click(object sender, EventArgs e)
        {
            ShareStatusTask status = new ShareStatusTask();
            status.Status = ViewModel.Note.Message;
            status.Show();
        }

        private void IconLeft_Click(object sender, EventArgs e)
        {
            ViewModel.PreviousNote();
            EnableArrow();
        }

        private void IconRight_Click(object sender, EventArgs e)
        {
            ViewModel.NextNote();
            EnableArrow();
        }

        private void EnableArrow()
        {
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = (ViewModel.SelectedIndex != 0);
            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = ViewModel.SelectedIndex != ViewModel.NoteList.Count - 1; 
        }

    }
}