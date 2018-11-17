using System;
using System.Collections.Generic;

namespace ActivateAnything
{
    /// <summary>
    ///     <para>
    ///         Attributes inheriting from this class will be used as rules by <see cref="Construct" />/>
    ///         when constructing a concrete instance
    ///     </para>
    ///     Rules inheriting from <see cref="IActivateConcreteTypeRule" /> are concerned
    ///     with where to look (e.g. which assemblies or namespaces) for a concrete type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class ActivateConcreteTypeRuleAttribute : Attribute,
        IActivateConcreteTypeRule,
        IActivateConcreteTypeByNameRule
    {
        public abstract Type FindTypeAssignableTo(
            string typeName,
            IEnumerable<Type> typesWaitingToBeBuilt = null,
            object searchAnchor = null);

        /// <summary>
        ///     Implementing subclasses should attempt to find a concrete type, assignable to <paramref name="type" /> by
        ///     following the rule which it (the subclass) names.
        /// </summary>
        /// <param name="type">The abstract Type for which we are now trying to build a concrete instance.</param>
        /// <param name="typesWaitingToBeBuilt">
        ///     The type which we were ultimately trying to build, and the types
        ///     we need to build it, which has recursively led us to need an instance of <paramref name="type" />.
        /// </param>
        /// <param name="searchAnchor"></param>
        /// <returns>
        ///     <list type="table">
        ///         <item>A concrete <see cref="Type" /> which is assignable to <see cref="type" />.</item>
        ///         <item>Returns null if the rule can identify no suitable <see cref="Type" />.</item>
        ///     </list>
        /// </returns>
        public abstract Type FindTypeAssignableTo(
            Type type,
            IEnumerable<Type> typesWaitingToBeBuilt = null,
            object searchAnchor = null);
    }
}