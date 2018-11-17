using System;
using System.Collections.Generic;
using System.Linq;

namespace ActivateAnything
{
    /// <summary>
    ///     This strategy will look only in the Assembly containing the <c>searchAnchor</c>.
    ///     If no <c>searchAnchor</c> was specified, search only in the Assembly containing the <c>Type</c> to be instantiated.
    ///     TODO: Split this into FindInAnchorAssembly and FindInSameAssembly
    /// </summary>
    public class FindInAnchorAssemblyAttribute : ActivateConcreteTypeRuleAttribute
    {
        public override Type FindTypeAssignableTo(
            Type type,
            IEnumerable<Type> typesWaitingToBeBuilt = null,
            object testFixtureType = null)
        {
            return FindTypeAssignableTo(testFixtureType, t => !t.IsAbstract && !t.IsInterface && type.IsAssignableFrom(t));
        }

        public override Type FindTypeAssignableTo(
            string typeNameRightPart,
            IEnumerable<Type> typesWaitingToBeBuilt = null,
            object searchAnchor = null)
        {
            return FindTypeAssignableTo(searchAnchor,
                t => !t.IsAbstract && !t.IsInterface && t.FullName.EndsWith(typeNameRightPart));
        }

        static Type FindTypeAssignableTo(object testFixtureType, Func<Type, bool> filterBy)
        {
            if (testFixtureType == null) return null;
            //
            return testFixtureType.GetType().Assembly.GetTypes().FirstOrDefault(filterBy);
        }
    }
}