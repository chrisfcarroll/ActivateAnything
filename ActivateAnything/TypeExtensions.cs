using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ActivateAnything
{
    public static class TypeExtensions
    {
        /// <summary>Find all <see cref="IActivateAnythingRule"/>s amongst the <see cref="MemberInfo.CustomAttributes"/> of
        /// <paramref name="typeWithAttributes"/>, including inherited attributes.</summary>
        /// <param name="typeWithAttributes"></param>
        /// <returns>All <see cref="IActivateAnythingRule"/>s found in the <paramref name="typeWithAttributes"/>'s Attributes</returns>
        public static IEnumerable<IActivateAnythingRule> GetActivateAnythingRuleAttributes(this Type typeWithAttributes)
        {
            return typeWithAttributes
                .GetCustomAttributes(typeof(IActivateAnythingRule), inherit: true)
                .Cast<IActivateAnythingRule>();
        }
    }
}