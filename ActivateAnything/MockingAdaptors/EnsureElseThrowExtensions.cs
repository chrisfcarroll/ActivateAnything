using System;

namespace ActivateAnything
{
    public static class EnsureElseThrowExtensions
    {
        public static T Ensure<T>(this T @this, Func<T, bool> assertion, Exception failureException)
        {
            if (!assertion(@this)) { throw failureException;}
            return @this;
        }

        public static T Ensure<T>(this T @this, Func<T, bool> assertion, string failureMessage)
        {
            if (!assertion(@this)) { throw new Exception(failureMessage); }
            return @this;
        }


        public static T EnsureNotNull<T>(this T @this, string failureMessage)
        {
            if (@this==null) { throw new Exception(failureMessage); }
            return @this;
        }
        public static T EnsureNotNull<T>(this T @this, Exception failureException)
        {
            if (@this == null) { throw failureException ; }
            return @this;
        }
    }
}