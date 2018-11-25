using System;

namespace ActivateAnything
{
    /// <summary><c>Assert (condition) else throw</c>Extensions</summary>
    public static class EnsureElseThrowExtensions
    {
        /// <summary>Throw <paramref name="failureException"/> if <paramref name="assertion"/> fails.</summary>
        /// <param name="this"></param>
        /// <param name="assertion"></param>
        /// <param name="failureException"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T Ensure<T>(this T @this, Func<T, bool> assertion, Exception failureException)
        {
            if (!assertion(@this)) throw failureException;
            return @this;
        }

        /// <summary>Throw an <c>Exception</c> with <paramref name="failureMessage"/> if
        /// <paramref name="assertion"/> fails.</summary>
        /// <param name="this"></param>
        /// <param name="assertion"></param>
        /// <param name="failureMessage"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T Ensure<T>(this T @this, Func<T, bool> assertion, string failureMessage)
        {
            if (!assertion(@this)) throw new Exception(failureMessage);
            return @this;
        }


        /// <summary>Throw an <c>Exception</c> with <paramref name="failureMessage"/> if
        /// <paramref name="@this"/> is Null</summary>
        /// <param name="this"></param>
        /// <param name="failureMessage"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T EnsureNotNull<T>(this T @this, string failureMessage)
        {
            if (@this == null) throw new Exception(failureMessage);
            return @this;
        }

        /// <summary>Throw <paramref name="failureException"/> if<paramref name="@this"/> is Null</summary>
        /// <param name="this"></param>
        /// <param name="failureException"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T EnsureNotNull<T>(this T @this, Exception failureException)
        {
            if (@this == null) throw failureException;
            return @this;
        }
    }
}