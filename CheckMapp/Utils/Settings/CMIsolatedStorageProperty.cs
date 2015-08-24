using System.IO.IsolatedStorage;

namespace CheckMapp.Utils.Settings
{
    /// <summary>
    /// This is wrapper class for storing one setting
    /// Object of this type must be single
    /// </summary>
    public class CMIsolatedStorageProperty<T>
    {
        private readonly object _defaultValue;
        private readonly string _name;
        private readonly object _syncObject = new object();

        public CMIsolatedStorageProperty(string name, T defaultValue = default(T))
        {
            _name = name;
            _defaultValue = defaultValue;
        }

        /// <summary>
        /// Determines if setting exists in the storage
        /// </summary>
        public bool Exists
        {
            get { return CMIsolatedStoragePropertyHelper.Store.Contains(_name); }
        }

        /// <summary>
        /// Use this property to access the actual setting value
        /// </summary>
        public T Value
        {
            get
            {
                if (!Exists)
                {
                    //Initializing only once
                    lock (_syncObject)
                    {
                        if (!Exists) SetDefault();
                    }
                }

                return (T)CMIsolatedStoragePropertyHelper.Store[_name];
            }
            set
            {
                CMIsolatedStoragePropertyHelper.Store[_name] = value;
                Save();
            }
        }

        private static void Save()
        {
            lock (CMIsolatedStoragePropertyHelper.ThreadLocker)
            {
                CMIsolatedStoragePropertyHelper.Store.Save();
            }
        }

        public void SetDefault()
        {
            Value = (T)_defaultValue;
        }
    }
}