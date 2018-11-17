using System;
using System.Collections.Generic;
using System.Reflection;

namespace ActivateAnything
{
    /// <summary>
    ///     <para>
    ///         Attributes inheriting from this class will be used as rules by <see cref="Construct" />/>
    ///         when constructing a concrete instance
    ///     </para>
    ///     Rules inheriting from <see cref="ChooseConstructorRuleAttribute" /> are concerned with
    ///     which constructor to choose (if there is more than one) when building a concrete type
    /// </summary>
    public abstract class ChooseConstructorRuleAttribute : Attribute, IChooseConstructorRule
    {
        public abstract ConstructorInfo ChooseConstructor(
            Type type,
            IEnumerable<Type> typesWaitingToBeBuilt,
            object searchAnchor = null);
    }
}