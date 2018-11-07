using System;
using System.Collections.Generic;
using System.Linq;

namespace ActivateAnything
{
    /// <summary>
    /// This strategy will look only in the Assembly containing the <c>anchorAssemblyType</c>.
    /// If no <c>anchorAssemblyType</c> was specified, search only in the Assembly containing the <c>Type</c> to be instantiated.
    /// 
    /// TODO: Split this into FindOnlyInAnchorAssembly and FindOnlyInSameAssembly
    /// 
    /// </summary>
    public class FindOnlyInAnchorAssemblyAttribute : ActivateAnythingFindConcreteTypeRuleAttribute 
    {
        public override Type FindTypeAssignableTo(Type type, IEnumerable<Type> typesWaitingToBeBuilt = null, object testFixtureType = null)
        {
            return FindTypeAssignableTo(testFixtureType, t => !t.IsAbstract && !t.IsInterface && type.IsAssignableFrom(t));
        }

        public override Type FindTypeAssignableTo(string typeNameRightPart, IEnumerable<Type> typesWaitingToBeBuilt = null, object anchorAssemblyType = null)
        {
            return FindTypeAssignableTo(anchorAssemblyType, t => !t.IsAbstract && !t.IsInterface && t.FullName.EndsWith(typeNameRightPart));
        }

        static Type FindTypeAssignableTo(object testFixtureType, Func<Type, bool> filterBy)
        {
            if (testFixtureType == null) { return null;}
            //
            return testFixtureType.GetType().Assembly.GetTypes().FirstOrDefault(filterBy);
        }
    }
}