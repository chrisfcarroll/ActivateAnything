using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ActivateAnything
{
    /// <summary>
    ///     <para>
    ///         Attributes inheriting from this class will be used as rules by <see cref="AnythingActivator.New" />
    ///         when constructing a concrete instance.
    ///     </para>
    ///     Rules inheriting from <see cref="ConstructorRule" /> are concerned with which constructor to
    ///     choose (if there is more than one) when building a concrete type.
    ///     This abstract baseclass also provides <see cref="VetoCircularDependency" /> to avoid recursion
    ///     leading to stack overflow.
    /// </summary>
    public abstract class ConstructorRule : Attribute, IChooseConstructorRule
    {
        /// <summary>
        ///     Choose a <see cref="ConstructorInfo" /> for <paramref name="type" />
        ///     or return null if this rule cannot provide one.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typesWaitingToBeBuilt"></param>
        /// <param name="searchAnchor"></param>
        /// <returns></returns>
        public abstract ConstructorInfo ChooseConstructor(
            Type              type,
            IEnumerable<Type> typesWaitingToBeBuilt,
            object            searchAnchor = null);

        /// <summary>
        ///     A veto to prevent infinite recursion. The default rule for veto is,
        ///     �Do not choose a <see cref="ConstructorInfo" /> which as one of its <see cref="MethodBase.GetParameters" />
        ///     requires a <c>Type</c> that is already in the stack of <c>Type</c> waiting to be built.�
        /// </summary>
        protected bool VetoCircularDependency(IEnumerable<Type> typesWaitingToBeBuilt, ConstructorInfo constructor)
        {
            return !constructor
                   .GetParameters()
                   .Any(p => typesWaitingToBeBuilt.Contains(p.ParameterType) && !p.IsOptional);
        }
    }
}
