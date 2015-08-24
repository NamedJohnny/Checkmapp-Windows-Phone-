using System;

namespace CheckMapp.Model.Utils
{
    public class Language
    {
        private Name name;

        public enum Name
        {
            English,
            German,
            French,
            Spanish
        };

        public Language()
        {
            this.name = Name.English;
        }

        public Language(Name name)
        {
            this.name = name;
        }

        public Name GetName()
        {
            return name;
        }

        public void SetName(Name name)
        {
            this.name = name;
        }
    }
}
