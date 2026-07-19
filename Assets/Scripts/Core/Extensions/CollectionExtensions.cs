using System;
using System.Collections.Generic;

namespace ZelaznaDroga.Core.Extensions
{
    /// <summary>
    /// Extension methods for collections.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Gets a value from a dictionary or returns a default value.
        /// </summary>
        public static TValue GetValueOrDefault<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary, 
            TKey key, 
            TValue defaultValue = default)
        {
            return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
        }

        /// <summary>
        /// Gets a value from a dictionary, adding it if it doesn't exist.
        /// </summary>
        public static TValue GetOrAdd<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary, 
            TKey key) where TValue : new()
        {
            if (!dictionary.TryGetValue(key, out TValue value))
            {
                value = new TValue();
                dictionary[key] = value;
            }
            return value;
        }

        /// <summary>
        /// Tries to add a value to a list if it doesn't already contain it.
        /// </summary>
        public static bool TryAddUnique<T>(this List<T> list, T item)
        {
            if (list.Contains(item)) return false;
            list.Add(item);
            return true;
        }

        /// <summary>
        /// Tries to remove a value from a list if it contains it.
        /// </summary>
        public static bool TryRemove<T>(this List<T> list, T item)
        {
            return list.Remove(item);
        }

        /// <summary>
        /// Swaps two elements in a list.
        /// </summary>
        public static void Swap<T>(this List<T> list, int index1, int index2)
        {
            if (index1 < 0 || index1 >= list.Count) return;
            if (index2 < 0 || index2 >= list.Count) return;
            
            T temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        /// <summary>
        /// Shuffles a list in place using Fisher-Yates algorithm.
        /// </summary>
        public static void Shuffle<T>(this List<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Returns a random element from the list.
        /// </summary>
        public static T Random<T>(this List<T> list)
        {
            if (list.Count == 0) throw new InvalidOperationException("Cannot get random element from empty list");
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        /// <summary>
        /// Tries to get a random element from the list.
        /// </summary>
        public static bool TryRandom<T>(this List<T> list, out T result)
        {
            if (list.Count == 0)
            {
                result = default;
                return false;
            }
            result = list.Random();
            return true;
        }
    }
}
