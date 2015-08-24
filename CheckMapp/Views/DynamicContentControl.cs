using CheckMapp.Model.Utils;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CheckMapp.Views
{
    public class DynamicContentControl : ContentControl
    {
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            this.ContentTemplate = DataTemplateSelector.GetTemplate(newContent as ViewModelBase);
        }
    }
}
