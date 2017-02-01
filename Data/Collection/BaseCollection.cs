using System;
using System.Collections;


namespace TinyRoar.Framework
{
    [Serializable]
    public abstract class BaseCollection : ICollection
    {

        public ArrayList Items = new ArrayList();

        // Empty Constructor for Serializable
        public BaseCollection()
        {
        }

        // Methods

        public abstract void Add(BaseElement element);

        public BaseElement this[int index]
        {
            get
            {
                return (BaseElement)Items[index];
            }
        }


        // Implement Interface for ICollection

        public void CopyTo(Array a, int index)
        {
            Items.CopyTo(a, index);
        }

        public int Count
        {
            get { return Items.Count; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public IEnumerator GetEnumerator()
        {
            return Items.GetEnumerator();
        }


    }

}