using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Countdown.ML.ExternalUtils
{
    /// <summary>
    /// Manages Frequency Distributions for items of type T
    /// </summary>
    /// <typeparam name="T">Type for item</typeparam>
    public class FrequencyDist<T>
    {
        /// <summary>
        /// Construct Frequency Distribution for the given list of items
        /// </summary>
        /// <param name="li">List of items to calculate for</param>
        public FrequencyDist(List<T> li)
        {
            CalcFreqDist(li);
        }

        /// <summary>
        /// Construct Frequency Distribution for the given list of items, across all keys in itemValues
        /// </summary>
        /// <param name="li">List of items to calculate for</param>
        /// <param name="itemValues">Entire list of itemValues to include in the frequency distribution</param>
        public FrequencyDist(List<T> li, List<T> itemValues)
        {
            CalcFreqDist(li);
            // add items to frequency distribution that are in itemValues but missing from the frequency distribution
            foreach (var v in itemValues)
            {
                if (!ItemFreq.Keys.Contains(v))
                {
                    ItemFreq.Add(v, new Item { value = v, count = 0 });
                }
            }
            // check that all values in li are in the itemValues list
            foreach (var v in li)
            {
                if (!itemValues.Contains(v))
                    throw new Exception(string.Format("FrequencyDist: Value in list for frequency distribution not in supplied list of values: '{0}'.", v));
            }
        }

        /// <summary>
        /// Calculate the frequency distribution for the values in list
        /// </summary>
        /// <param name="li">List of items to calculate for</param>
        void CalcFreqDist(List<T> li)
        {
            itemFreq = new SortedList<T, Item>((from item in li
                                                group item by item into theGroup
                                                select new Item { value = theGroup.FirstOrDefault(), count = theGroup.Count() }).ToDictionary(q => q.value, q => q));
        }
        SortedList<T, Item> itemFreq = new SortedList<T, Item>();

        /// <summary>
        /// Getter for the Item Frequency list
        /// </summary>
        public SortedList<T, Item> ItemFreq { get { return itemFreq; } }

        public int Freq(T value)
        {
            if (itemFreq.Keys.Contains(value))
            {
                return itemFreq[value].count;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns the list of distinct values between two lists
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <returns></returns>
        public static List<T> GetDistinctValues(List<T> l1, List<T> l2)
        {
            return l1.Concat(l2).ToList().Distinct().ToList();
        }

        /// <summary>
        /// Manages a count of items (int, string etc) for frequency counts
        /// </summary>
        /// <typeparam name="T">The type for item</typeparam>
        public class Item
        {
            /// <summary>
            /// The value of the item, e.g. int or string
            /// </summary>
            public T value { get; set; }
            /// <summary>
            /// The count of the item
            /// </summary>
            public int count { get; set; }
        }
    }
}
