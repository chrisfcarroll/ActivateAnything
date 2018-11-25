using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace ActivateAnything
{
    /// <summary>
    ///     This set of rules should do the right thing in many cases. It applies these rulesets:
    ///     <list type="bullet">
    ///         <item>
    ///             The rules in <see cref="DefaultFindConcreteTypeRuleSequence" /> in order, when looking for a concrete
    ///             type to instantiate an interface or abstract type
    ///         </item>
    ///         <item>The rules in <see cref="DefaultChooseConstructorRuleSequence" /> when constructing a concrete type.</item>
    ///     </list>
    /// </summary>
    public class ActivateDefaultRules : Attribute, IFindTypeRule, IChooseConstructorRule
    {
        /// <summary>
        ///     The default <see cref="IFindTypeRule" /> sequence for finding a type to instantiate is, in
        ///     this order:
        ///     <see cref="FindInAnyAssemblyReferencedByAssemblyContainingType" />,
        ///     <see cref="FindInAnchorAssembly" />,
        ///     <see cref="FindInDirectory" />
        /// </summary>
        public static readonly IList<IFindTypeRule>
            DefaultFindConcreteTypeRuleSequence =
                new ReadOnlyCollection<IFindTypeRule>(
                    new IFindTypeRule[]
                    {
                        new FindInAnyAssemblyReferencedByAssemblyContainingType(),
                        new FindInAnchorAssembly(),
                        new FindInDirectory()
                    });

        /// <summary>
        ///     The default <see cref="IChooseConstructorRule" /> is, in this order:
        ///     <see cref="ConstructorWithMostParametersRule" />,
        ///     <see cref="ConstructorWithFewestParametersRule" />
        /// </summary>
        public static readonly IList<IChooseConstructorRule>
            DefaultChooseConstructorRuleSequence =
                new ReadOnlyCollection<IChooseConstructorRule>(
                    new IChooseConstructorRule[]
                    {
                        new ConstructorWithMostParametersRule(),
                        new ConstructorWithFewestParametersRule()
                    });

        /// <summary>
        ///     The default ActivateAnything ruleset is the union of <see cref="DefaultFindConcreteTypeRuleSequence" /> and
        ///     <see cref="DefaultChooseConstructorRuleSequence" />
        /// </summary>
        public static readonly IReadOnlyCollection<IActivateAnythingRule>
            AllDefaultRules =
                new ReadOnlyCollection<IActivateAnythingRule>(
                    DefaultFindConcreteTypeRuleSequence.Union<IActivateAnythingRule>(
                            DefaultChooseConstructorRuleSequence)
                        .ToList());

        /// <summary>Use <see cref="DefaultChooseConstructorRuleSequence"/> to choose a constructor</summary>
        /// <param name="type"></param>
        /// <param name="typesWaitingToBeBuilt"></param>
        /// <param name="searchAnchor"></param>
        /// <returns>The <c>ConstructorInfo</c> is one is found, <c>null</c> if not</returns>
        public ConstructorInfo ChooseConstructor(Type type, IEnumerable<Type> typesWaitingToBeBuilt, object searchAnchor = null)
        {
            return AnythingActivator.Instance.ChooseConstructor(type,
                DefaultChooseConstructorRuleSequence,
                typesWaitingToBeBuilt,
                searchAnchor);
        }

        /// <summary>Use <see cref="AllDefaultRules"/> to find a <c>Type</c> which is assignable to <paramref name="type"/> 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typesWaitingToBeBuilt"></param>
        /// <param name="searchAnchor"></param>
        /// <returns>The <c>Type</c> if one is found, <c>null</c> if not.</returns>
        public Type FindTypeAssignableTo(Type type, IEnumerable<Type> typesWaitingToBeBuilt = null, object searchAnchor = null)
        {
            return TypeFinder.FindConcreteTypeAssignableTo(type, AllDefaultRules, typesWaitingToBeBuilt, searchAnchor);
        }
    }
}