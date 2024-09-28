#if UNITY_EDITOR
namespace AssetBundleBrowser
{
    using System.Collections.Generic;

    public static class STZAssetBundleBrowserExtension
    {
        #region IEnumerable
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer = null)
        {
            return new HashSet<T>(source, comparer);
        }
        #endregion

        #region List
        public static bool TryAdd<T>(this IList<T> list, T value)
        {
            if (list == null || list.Contains(value)) return false;
            list.Add(value);
            return true;
        }
        #endregion

        #region Dictionary
        public static TValue GetOrDefault<TKey, TValue>(
                this IDictionary<TKey, TValue> dictionary,
                TKey key,
                TValue defaultValue
            )
        {
            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public static TValue GetOrNull<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key
        ) where TValue : class
        {
            return dictionary.ContainsKey(key) ? dictionary[key] : null;
        }

        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null)
            {
                return false;
            }

            if (dictionary.ContainsKey(key) == false)
            {
                dictionary.Add(key, value);
                return true;
            }
            return false;
        }

        public static void AddOrAssign<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        public static void AddByList<TKey, TValue>(this Dictionary<TKey, List<TValue>> dict, TKey key, TValue value)
        {
            if (dict.TryGetValue(key, out var list))
            {
                list.Add(value);
            }
            else
            {
                dict.Add(key, new List<TValue> { value });
            }
        }
        #endregion

        /// <summary>
        /// 해당 컬렉션이 null 이거나 비어있는지 확인
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            if (collection != null)
            {
                return collection.Count == 0;
            }

            return true;
        }

        /// <summary>
        /// string.IsNullOrEmpty 이거나 문자가 "null" 혹은 "none" 인지 확인
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return true;
            }

            string text = str.ToLower();
            if (text.Equals("null") || text.Equals("none"))
            {
                return true;
            }

            return false;
        }
    }
}
#endif