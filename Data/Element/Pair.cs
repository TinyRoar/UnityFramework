using System.Collections;
using System;


namespace TinyRoar.Framework
{
    [Serializable]
    public class Pair : BaseElement
    {
        public static DataManagement Management = null;

        public string Key;
        private string value;
        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;

                // use this only after first/default deserization finished
                if (Management != null)
                    Management.Set(this);
            }
        }

        public Pair()
        {
        }

        public Pair(string key)
        {
            this.Key = key;
            this.value = "0";
        }

        public Pair(string key, string value)
        {
            this.Key = key;
            this.value = value;
        }

        public Pair(string key, int value)
        {
            this.Key = key;
            this.value = value.ToString();
        }

        public Pair(string key, bool value)
        {
            this.Key = key;
            this.value = (value) ? "1" : "0";
        }

        public int Int
        {
            get
            {
                try
                {
                    return Convert.ToInt32(this.Value);
                }
                catch
                {
                    UnityEngine.Debug.LogWarning("Data-Key " + Key + " isn't an int :'(");
                }
                return 0;
            }
        }

        public Int64 Int64
        {
            get
            {
                try
                {
                    return Convert.ToInt64(this.Value);
                }
                catch
                {
                    UnityEngine.Debug.LogWarning("Data-Key " + Key + " isn't an int :'(");
                }
                return 0;
            }
        }

        public bool IsInt
        {
            get
            {
                try
                {
                    Convert.ToInt32(this.value);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool Bool
        {
            get
            {
                if (this.value.ToString() != "0" && this.value.ToString().ToLower() != "false")
                    return true;
                return false;
            }
        }


    }

}