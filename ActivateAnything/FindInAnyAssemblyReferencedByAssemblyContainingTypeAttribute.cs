using System;
using System.Collections.Generic;
using System.Linq;

namespace ActivateAnything
{
    /// <summary>
    ///     <strong>Note</strong> that this strategy may search not one but many assemblies:
    ///     <list type="bullet">
    ///         <item>The 'Assembly Under Test' is primarily the assembly containing the Type under test.</item>
    ///         <item>
    ///             But there are also the assemblies containing the Types which we recursively need in
    ///             under to construct the Type under test.
    ///         </item>
    ///     </list>
    /// </summary>
    public class FindInAnyAssemblyReferencedByAssemblyContainingTypeAttribute : ActivateAnythingFindConcreteTypeRuleAttribute
    {
        public override Type FindTypeAssignableTo(
            Type type,
            IEnumerable<Type> typesWaitingToBeBuilt = null,
            object getType = null)
        {
            return FindTypeAssignableTo(type,
                typesWaitingToBeBuilt,
                t => !t.IsAbstract && !t.IsInterface && type.IsAssignableFrom(t));
        }

        static Type FindTypeAssignableTo(Type type, IEnumerable<Type> typesWaitingToBeBuilt, Func<Type, bool> filterBy)
        {
            return type
                .Assembly.GetTypes()
                .Union((typesWaitingToBeBuilt ?? new Type[0]).SelectMany(t => t.Assembly.GetTypes()))
                .FirstOrDefault(filterBy);
        }

        public override Type FindTypeAssignableTo(
            string typeName,
            IEnumerable<Type> typesWaitingToBeBuilt = null,
            object searchAnchor = null)
        {
            throw new InvalidOperationException(GetType()
                                                + " cannot find a a Type by name because 'AssemblyUnderTest' is taken to mean the assembly in which the target type is found.");
        }
    }
}