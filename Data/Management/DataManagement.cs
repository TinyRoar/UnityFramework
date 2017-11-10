using System.Collections;
using UnityEngine;


namespace TinyRoar.Framework
{
    public class DataManagement : BaseManagement<DataManagement>
    {

        public DataManagement()
        {
            // set path and filename without extension
            SetPath("gamedata");
            Management = ManagementType.Pair;

            // Deserialize DataStorage
            this.Deserialize<PairCollection>(typeof(Pair));

            // set reference to this Management for Updating
            // important -> set this after first/default deserization finished
            Pair.Management = this;
        }

        // Add Item to Collection
        public void Set(Pair pair)
        {
            // check against error
            if (Collection == null)
            {
                UnityEngine.Debug.Log("PairCollection not init");
                return;
            }

            // Add to collection
            Collection.Add(pair);

            // Notify for debug
            //UnityEngine.Debug.Log("Serialize " + pair.Key + " ("+pair.Value+")");

            // save as xml/binary data
            doSaving = true;
        }

        public Pair Set(string key, string value)
        {
            Pair pair = new Pair(key, value);
            this.Set(pair);
            return pair;
        }

        public Pair Set(string key, object value)
        {
            Pair pair = new Pair(key, value.ToString());
            this.Set(pair);
            return pair;
        }

        public Pair Set(string key, int value)
        {
            Pair pair = new Pair(key, value);
            this.Set(pair);
            return pair;
        }

        public Pair Set(string key, bool value)
        {
            Pair pair = new Pair(key, value);
            this.Set(pair);
            return pair;
        }

        public Pair Get(string key, string defaultValue)
        {
            if(this.CheckItem(key))
                return (this.Collection as PairCollection).Get(new Pair(key));
            return new Pair(key, defaultValue);
        }

        public Pair Get(string key, int defaultValue)
        {
            return this.Get(key, defaultValue.ToString());
        }

        public Pair Get(string key)
        {
            return (this.Collection as PairCollection).Get(new Pair(key));
        }

        public Pair Get(Pair pair)
        {
            return (this.Collection as PairCollection).Get(pair);
        }

        public bool CheckItem(string key)
        {
            return (this.Collection as PairCollection).CheckItem(key);
        }

        public int Increase(string key)
        {
            int newVal = (this.Collection as PairCollection).Increase(new Pair(key));
            doSaving = true;
            return newVal;
        }

        public int Increase(Pair pair)
        {
            int newVal = (this.Collection as PairCollection).Increase(pair);
            doSaving = true;
            return newVal;
        }

        public void Remove(string key)
        {
            if((this.Collection as PairCollection).Remove(new Pair(key)))
                doSaving = true;
        }

        public void Remove(Pair pair)
        {
            if((this.Collection as PairCollection).Remove(pair))
                doSaving = true;
        }

    }

}