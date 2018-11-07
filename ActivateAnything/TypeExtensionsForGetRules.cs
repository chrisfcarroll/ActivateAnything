using System;
using System.Collections.Generic;
using System.Linq;

namespace ActivateAnything
{
    public static class TypeExtensionsForGetRules
    {
        public static IEnumerable<IActivateAnythingRule> GetActivateAnythingRulesFromAttributes(this Type typeWithAttributes)
        {
            return typeWithAttributes
                .GetCustomAttributes(typeof(IActivateAnythingRule), inherit: true)
                .Cast<IActivateAnythingRule>();
        }
    }
}