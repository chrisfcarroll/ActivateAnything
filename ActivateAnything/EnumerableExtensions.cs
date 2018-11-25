using System.Collections.Generic;
using System.Linq;

namespace ActivateAnything
{
    public static class EnumerableExtensions
    {
        /// <summary>Returns <paramref name="right" />.<see cref="Enumerable.Union{TSource}" />(<paramref name="left" />)</summary>
        /// <param name="left">The  items to append to <paramref name="right" /></param>
        /// <param name="right">The items after which <paramref name="left" /> are to be appended</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        ///     <c>right.Union(left)</c>
        /// </returns>
        public static IEnumerable<T> After<T>(this IEnumerable<T> left, IEnumerable<T> right) { return right.Union(left); }

        /// <summary>Returns <paramref name="right" />.<see cref="Enumerable.Union{TSource}" />(<paramref name="left" />)</summary>
        /// <param name="left">The  items to append to <paramref name="right" /></param>
        /// <param name="right">The items after which <paramref name="left" /> are to be appended</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        ///     <c>right.Union(left)</c>
        /// </returns>
        public static IEnumerable<T> After<T>(this IEnumerable<T> left, params T[] right) { return right.Union(left); }

        /// <summary>
        ///     An overload for
        ///     <see
        ///         cref="Enumerable.Union{TSource}(System.Collections.Generic.IEnumerable{TSource},System.Collections.Generic.IEnumerable{TSource})" />
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        ///     <c>left.Union(right)</c>
        /// </returns>
        public static IEnumerable<T> Union<T>(this IEnumerable<T> left, params T[] right)
        {
            return Enumerable.Union(left, right);
        }

        /// <summary>Return <c>item.Union(more)</c></summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="more"></param>
        /// <returns>
        ///     <c>item.Union(more)</c>
        /// </returns>
        public static IEnumerable<T> And<T>(this IEnumerable<T> items, params T[] more) { return items.Union(more); }
    }
}
