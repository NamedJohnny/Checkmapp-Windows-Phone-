using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckMapp.Utils.Languages
{
    internal class LocalizationChangedEventArgs : EventArgs
    {
        public string NewLocalization { get; private set; }

        public LocalizationChangedEventArgs(string newLocalization)
        {
            NewLocalization = newLocalization;
        }
    }
}
