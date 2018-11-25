using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ActivateAnything
{
    public static class TypeExtensions
    {
        /// <summary>
        ///     Find all <see cref="IActivateAnythingRule" />s amongst the <see cref="MemberInfo.CustomAttributes" /> of
        ///     <paramref name="typeWithAttributes" />, including inherited attributes.
        /// </summary>
        /// <param name="typeWithAttributes"></param>
        /// <returns>All <see cref="IActivateAnythingRule" />s found in the <paramref name="typeWithAttributes" />'s Attributes</returns>
        public static IEnumerable<IActivateAnythingRule> GetActivateAnythingRuleAttributes(this Type typeWithAttributes)
        {
            return typeWithAttributes
            .GetCustomAttributes(typeof(IActivateAnythingRule), true)
            .Cast<IActivateAnythingRule>();
        }

        /// <summary>Returns <c>default(T)</c> where typeof(T) is <paramref name="type" /></summary>
        /// <param name="type"></param>
        /// <returns>For Reference Types, returns <c>null</c>. For Value types, the default instance.</returns>
        public static object DefaultValue(this Type type)
        {
            return
            type.IsValueType
            ? Expression.Lambda<Func<object>>(Expression.Default(type)).Compile()()
            : null;
        }
    }
}
