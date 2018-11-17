using System.Collections.Generic;
using System.Linq;

namespace ActivateAnything
{
    public static class EnumerableCombineExtensions
    {
        /// <summary>Return <c>item.Union(more)</c></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="more"></param>
        /// <returns><c>item.Union(more)</c></returns>
        public static IEnumerable<T> And<T>(this IEnumerable<T> items, params T[] more) => items.Union(more);
    }
}