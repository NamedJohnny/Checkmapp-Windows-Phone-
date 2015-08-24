using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using WindowsPhone.MVVM.Tombstone;

namespace CheckMapp.Views
{
    public class BasePage : PhoneApplicationPage
    {
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            TombstoneHelper.page_OnNavigatedTo(this, e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            TombstoneHelper.page_OnNavigatedFrom(this, e);
        }
    }
}
