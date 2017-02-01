using System.Collections;
using System;
using System.Runtime.Serialization;


namespace TinyRoar.Framework
{
    [Serializable]
    public class PairCollection : BaseCollection
    {

        // Need empty Constructor for Serializable
        public PairCollection()
        {
        }

        public Pair Get(Pair searchedPair)
        {
            foreach (Pair element in Items)
            {
                if (element.Key == searchedPair.Key)
                {
                    return element;
                }
            }
            return new Pair(searchedPair.Key);
        }

        public override void Add(BaseElement element)
        {
            Pair pairNew = element as Pair;

            // check existsing item and remove it
            Remove(pairNew);

            // add
            Items.Add(pairNew);
        }

        public bool CheckItem(string key)
        {
            foreach (Pair element in Items)
                if (element.Key == key)
                {
                    return true;
                }
            return false;
        }

        public int Increase(Pair searchedPair)
        {
            Pair pair = Get(searchedPair);

            if (pair.IsInt)
            {
                int newVal = pair.Int + 1;
                pair.Value = newVal.ToString();
                return pair.Int;
            }

            return 0;
        }

        public bool Remove(Pair pair)
        {
            // check existsing item and remove it
            foreach (Pair item in Items)
            {
                if (item.Key == pair.Key)
                {
                    Items.Remove(item);
                    return true;
                }
            }
            return false;
        }

    }

}