using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System
{
    public static class IEnumerableExtensions
    {
        public static T Random<T>(this IEnumerable<T> collection) where T : class =>
            collection is IList<T> asList
                ? asList.Count == 0
                    ? null
                    : asList[new Random().Next(0, asList.Count)]
                : collection.ToList().Random();

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> input) =>
            input.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static bool Contains<T>(this IEnumerable<T> list, Predicate<T> predicate)
        {
            foreach (var element in list)
                if (predicate(element))
                    return true;
            return false;
        }
            
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var element in list)
                action(element);
        }

        public static void ForEach<T>(this IEnumerable<T> list, Action<T, int> action)
        {
            var counter = 0;
            foreach (var element in list)
                action(element, counter++);
        }

        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> enumerable)
        {
            var list = new List<T>();
            await foreach (var item in enumerable)
                list.Add(item);
            return list;
        }

        public static async IAsyncEnumerable<List<T>> ToListsAsync<T>(this IAsyncEnumerable<T> enumerable, int batchSize)
        {
            var list = new List<T>(batchSize);
            await foreach (var item in enumerable)
            {
                list.Add(item);
                if (list.Count == batchSize)
                {
                    yield return list;
                    list = new List<T>(batchSize);
                }
            }
            if (list.Count > 0)
                yield return list;
        }

        public static IEnumerable<T> EnumerateSynchronously<T>(this IAsyncEnumerable<T> enumerable)
        {
            var enumerator = enumerable.GetAsyncEnumerator();
            while (enumerator.MoveNextAsync().AsTask().Result)
                yield return enumerator.Current;
        }
    }
}
