using System;
using System.Collections.Generic;
using System.Linq;

namespace ActivateAnything
{
    public static class TypeFinder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of which an instance (possibly of a subclass) is desired</param>
        /// <param name="rules">The <see cref="IActivateAnythingRule"/> rules for where to search for Types</param>
        /// <param name="typesWaitingToBeBuilt"></param>
        /// <param name="searchAnchor"></param>
        /// <returns>A <see cref="Type"/>, if one is founnd or null if not.</returns>
        public static Type FindConcreteTypeAssignableTo(Type type, IEnumerable<IActivateAnythingRule> rules, IEnumerable<Type> typesWaitingToBeBuilt, object searchAnchor)
        {
            var result = rules
                .OfType<IActivateAnythingFindConcreteTypeRule>()
                .Select(r => r.FindTypeAssignableTo(type, typesWaitingToBeBuilt, searchAnchor))
                .FirstOrDefault(t => t != null);
            return result;
        }
    }
}