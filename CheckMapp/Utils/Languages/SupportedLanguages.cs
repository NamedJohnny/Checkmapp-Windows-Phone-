using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckMapp.Resources;

namespace CheckMapp.Utils.Languages
{
    internal static class SupportedLanguages
    {
        public const string En = "en";
        public const string Fr = "fr";

        public static Dictionary<string, string> langDictionary = new Dictionary<string, string>()
	    {
	        {En, AppResources.Lang_English},
	        {Fr, AppResources.Lang_French}
	    };   
    }
}
