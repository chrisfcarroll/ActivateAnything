using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace ActivateAnything
{
    /// <summary>
    /// This set of rules should do the right thing in many cases. It applies these rulesets: 
    /// <list type="bullet">
    /// <item>The rules in <see cref="DefaultFindConcreteTypeRuleSequence"/> in order, when looking for a concrete type to instantiate an interface or abstract type</item>
    /// <item>The rules in <see cref="DefaultChooseConstructorRuleSequence"/> when constructing a concrete type.</item>
    /// </list>
    /// </summary>
    public class ActivateAnythingDefaultRulesAttribute : Attribute, IActivateAnythingFindConcreteTypeRule, IActivateAnythingChooseConstructorRule
    {
        public Type FindTypeAssignableTo(Type type, IEnumerable<Type> typesWaitingToBeBuilt = null, object searchAnchor = null)
        {
            return TypeFinder.FindConcreteTypeAssignableTo(type, AllDefaultRules, typesWaitingToBeBuilt, searchAnchor);
        }

        public ConstructorInfo ChooseConstructor(Type type, IEnumerable<Type> typesWaitingToBeBuilt, object searchAnchor = null)
        {
            return CreateInstance.ChooseConstructor(type, DefaultChooseConstructorRuleSequence, typesWaitingToBeBuilt, searchAnchor);
        }

        /// <summary>
        /// The default <see cref="IActivateAnythingFindConcreteTypeRule"/> sequence for finding a type to instantiate is, in this order:
        /// <see cref="FindInAnyAssemblyReferencedByAssemblyContainingTypeAttribute"/>, 
        /// <see cref="FindOnlyInAnchorAssemblyAttribute"/>, 
        /// <see cref="FindInAssembliesInAppDomainBaseDirectoryAttribute"/>
        /// </summary>
        public static readonly IList<IActivateAnythingFindConcreteTypeRule> 
                                   DefaultFindConcreteTypeRuleSequence = 
                                           new ReadOnlyCollection<IActivateAnythingFindConcreteTypeRule>(
                                               new IActivateAnythingFindConcreteTypeRule[]{ 
                                                   new FindInAnyAssemblyReferencedByAssemblyContainingTypeAttribute(), 
                                                   new FindOnlyInAnchorAssemblyAttribute(), 
                                                   new FindInAssembliesInAppDomainBaseDirectoryAttribute() });

        /// <summary>
        /// The default <see cref="IActivateAnythingChooseConstructorRule"/> is, in this order:
        /// <see cref="ChooseConstructorWithMostParametersAttribute"/>,
        /// <see cref="ChooseConstructorWithFewestParametersAttribute"/>
        /// </summary>
        public static readonly IList<IActivateAnythingChooseConstructorRule>
                                   DefaultChooseConstructorRuleSequence =
                                           new ReadOnlyCollection<IActivateAnythingChooseConstructorRule>(
                                               new IActivateAnythingChooseConstructorRule[]{ 
                                                   new ChooseConstructorWithMostParametersAttribute(),
                                                   new ChooseConstructorWithFewestParametersAttribute() });

        /// <summary>
        /// The default ActivateAnything ruleset is the union of <see cref="DefaultFindConcreteTypeRuleSequence"/> and <see cref="DefaultChooseConstructorRuleSequence"/>
        /// </summary>
        public static readonly IEnumerable<IActivateAnythingRule> 
                                    AllDefaultRules = 
                                        new ReadOnlyCollection<IActivateAnythingRule>(
                                                (DefaultFindConcreteTypeRuleSequence.
                                                    Union<IActivateAnythingRule>(
                                                        DefaultChooseConstructorRuleSequence)
                                                ).ToList());

    }
}