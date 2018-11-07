using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ActivateAnything
{
    public class CreateInstance
    {
        /// <summary>
        /// Create an instance of something assignable to <typeparamref name="T"/>. 
        /// Do so using the <see cref="IActivateAnythingRule"/> rules found in the <see cref="Attribute"/>s 
        /// of <paramref name="searchAnchor"/>.
        /// </summary>
        /// <param name="anchorAndRuleProvider">an object whose <c>Type</c> will be used both as <c>searchAnchor</c>
        /// and as <c>ruleProvider</c>
        /// <list type="bullet">
        /// <item><description>The <see cref="IActivateAnythingRule"/> rules will be those found as <see cref="Type.Attributes"/> 
        /// of <c>anchorAndRuleProvider.GetType()</c>. If the object has no <see cref="IActivateAnythingRule"/> attributes, 
        /// then the <see cref="DefaultRules"/> will be used.</description></item>
        /// <item><description>The <see cref="Type"/> and especially the <see cref="Type.Assembly"/> will be used as the 
        /// "searchAnchor". Some rules use the searchAnchor as a reference point—whether as a starting point or as a 
        /// limit—to their search. For instance, the <see cref="FindInAnchorAssemblyAttribute"/> rule will only look for 
        /// concrete types in the anchor Assembly.
        /// </description></item>
        /// </list>
        /// </param>
        /// <returns>An instance of type <typeparamref name="T"/></returns>
        public static T Of<T>(object anchorAndRuleProvider)
        {
            if(anchorAndRuleProvider==null)return Of<T>();

            return (T)Of(typeof(T), 
                         anchorAndRuleProvider.GetType().GetActivateAnythingRulesFromAttributes(), 
                         typesWaitingToBeBuilt:null, 
                         searchAnchor: anchorAndRuleProvider);
        }

        /// <summary>
        /// Creates an instance of something assignable to <typeparamref name="T"/> using the given <paramref name="rules"/>.
        /// Note that since this overload doesn't take an <c>anchor</c> parameter, rules such as 
        /// <see cref="FindInAnchorAssemblyAttribute"/> which depend on an anchor will not run.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rules"><see cref="IActivateAnythingRule"/> rules are of three kinds:
        /// <list type="bullet">
        /// <item><see cref="IActivateAnythingCreateInstanceRule"/> provides an immediate source of a concrete
        /// type. For instance, the <see cref="CreateFromFactoryMethodAttribute"/> rule.</item>
        /// <item><see cref="IActivateAnythingFindConcreteTypeRule"/> provides rules for where to look for candidate 
        /// concrete subTypes of <typeparamref name="T"/>.</item>
        /// <item><see cref="IActivateAnythingChooseConstructorRule"/> rules for how to choose between constructors when
        /// the <see cref="IActivateAnythingFindConcreteTypeRule"/>s have found a <c>Type</c> to instantiate.</item>
        /// </list>
        /// If<paramref name="rules"/> is null, the <see cref="DefaultRules"/> will be used.
        /// </param>
        /// <returns>An instance of type <typeparamref name="T"/></returns>
        public static T Of<T>(IEnumerable<IActivateAnythingRule> rules= null)
        {
            return (T)Of(typeof(T), rules, null);
        }

        /// <summary>
        /// Creates an instance of something assignable to <typeparamref name="T"/>. Does so using the given 
        /// <paramref name="rules"/>.
        /// </summary>
        /// <param name="rules"><see cref="IActivateAnythingRule"/> rules are of three kinds:
        /// <list type="bullet">
        /// <item><see cref="IActivateAnythingCreateInstanceRule"/> provides an immediate source of a concrete
        /// type. For instance, the <see cref="CreateFromFactoryMethodAttribute"/> rule.</item>
        /// <item><see cref="IActivateAnythingFindConcreteTypeRule"/> provides rules for where to look for candidate 
        /// concrete subTypes of <typeparamref name="T"/>.</item>
        /// <item><see cref="IActivateAnythingChooseConstructorRule"/> rules for how to choose between constructors when
        /// the <see cref="IActivateAnythingFindConcreteTypeRule"/>s have found a <c>Type</c> to instantiate.</item>
        /// </list>
        /// If<paramref name="rules"/> is null, the <see cref="DefaultRules"/> will be used.
        /// </param>
        /// <param name="searchAnchor">The <see cref="Type"/> and especially the <see cref="Type.Assembly"/> of <c>searchAnchor</c> may
        /// be used by some rules as a reference point—whether as a starting point or as a limit—to their 
        /// search. For instance, the <see cref="FindInAnchorAssemblyAttribute"/> rule will only look for concrete types in the anchor Assembly.</param>
        /// <returns>An instance of type <typeparamref name="T"/></returns>
        public static T Of<T>(IEnumerable<IActivateAnythingRule> rules, object searchAnchor)
        {
            return (T) Of(typeof (T), rules, typesWaitingToBeBuilt:null, searchAnchor: searchAnchor);
        }

        /// <summary>
        /// Creates an instance of something assignable to <paramref name="type"/> using the given <paramref name="rules"/>
        /// and <paramref name="searchAnchor"/>
        /// </summary>
        /// <param name="type">The type of which a concrete instance is wanted.</param>
        /// <param name="rules"><see cref="IActivateAnythingRule"/> rules are of three kinds:
        /// <list type="bullet">
        /// <item><see cref="IActivateAnythingCreateInstanceRule"/> provides an immediate source of a concrete
        /// type. For instance, the <see cref="CreateFromFactoryMethodAttribute"/> rule.</item>
        /// <item><see cref="IActivateAnythingFindConcreteTypeRule"/> provides rules for where to look for candidate 
        /// concrete subTypes of <typeparamref name="T"/>.</item>
        /// <item><see cref="IActivateAnythingChooseConstructorRule"/> rules for how to choose between constructors when
        /// the <see cref="IActivateAnythingFindConcreteTypeRule"/>s have found a <c>Type</c> to instantiate.</item>
        /// </list>
        /// If<paramref name="rules"/> is null, the <see cref="DefaultRules"/> will be used.
        /// </param>
        /// <param name="typesWaitingToBeBuilt">The 'stack' of types we are trying to build grows as instantiating a type 
        /// recursively requires the instantion of its constructor dependencies.
        /// This parameter is for the benefit of recursive rules whose strategy may vary depending on what we're trying to build.
        /// </param>
        /// <param name="searchAnchor">The <see cref="Type"/> and especially the <see cref="Type.Assembly"/> of <c>searchAnchor</c> may
        /// be used by some rules as a reference point—whether as a starting point or as a limit—to their 
        /// search. For instance, the <see cref="FindInAnchorAssemblyAttribute"/> rule will only look for concrete types in the anchor Assembly.</param>        /// <returns>An instance of type <paramref name="type"/> if possible, null if unable to construct one.</returns>
        public static object Of(Type type, IEnumerable<IActivateAnythingRule> rules=null, IEnumerable<Type> typesWaitingToBeBuilt = null, object searchAnchor = null)
        {
            rules = rules ?? DefaultRules;
            typesWaitingToBeBuilt = (typesWaitingToBeBuilt ?? new List<Type>()).Union(new[] { type });

            var customRuleResult=rules.OfType<IActivateAnythingCreateInstanceRule>()
                .Select(r => r.CreateInstance(type, typesWaitingToBeBuilt, searchAnchor))
                .FirstOrDefault();

            if(customRuleResult!=null)
            {
                return customRuleResult;
            }
            else if (type.IsAbstract || type.IsInterface)
            {
                return Of(TypeFinder.FindConcreteTypeAssignableTo(type, rules, typesWaitingToBeBuilt, searchAnchor), rules, typesWaitingToBeBuilt, searchAnchor);
            }
            else if (type == typeof(string))
            {
                return typeof(string).Name;
            }
            else if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                return InstanceFromConstructorRules(type, rules, typesWaitingToBeBuilt, searchAnchor);
            }
        }

        /// <summary>
        /// Use <see cref="Activator.CreateInstance"/> to activate an Instance of the concrete type <paramref name="type"/>.
        /// Use the <see cref="IActivateAnythingChooseConstructorRule"/> to choose a constructor.
        /// If the chosen constructor has dependencies, then recursively use 
        /// <see cref="Of(Type, IEnumerable{IActivateAnythingRule}, IEnumerable{Type}, object)"/> to create the dependencies.
        /// </summary>
        /// <param name="type">This should be a concrete type, otherwise activation will fail.</param>
        /// <param name="rules"><see cref="IActivateAnythingRule"/> rules are of three kinds:
        /// <list type="bullet">
        /// <item><see cref="IActivateAnythingCreateInstanceRule"/> provides an immediate source of a concrete
        /// type. For instance, the <see cref="CreateFromFactoryMethodAttribute"/> rule.</item>
        /// <item><see cref="IActivateAnythingFindConcreteTypeRule"/> provides rules for where to look for candidate 
        /// concrete subTypes of <typeparamref name="T"/>.</item>
        /// <item><see cref="IActivateAnythingChooseConstructorRule"/> rules for how to choose between constructors when
        /// the <see cref="IActivateAnythingFindConcreteTypeRule"/>s have found a <c>Type</c> to instantiate.</item>
        /// </list>
        /// If<paramref name="rules"/> is null, the <see cref="DefaultRules"/> will be used.
        /// </param>
        /// <param name="typesWaitingToBeBuilt">The 'stack' of types we are trying to build grows as instantiating a type 
        /// recursively requires the instantion of its constructor dependencies.
        /// This parameter is for the benefit of recursive rules whose strategy may vary depending on what we're trying to build.
        /// </param>
        /// <param name="searchAnchor">The <see cref="Type"/> and especially the <see cref="Type.Assembly"/> of <c>searchAnchor</c> may
        /// be used by some rules as a reference point—whether as a starting point or as a limit—to their 
        /// search. For instance, the <see cref="FindInAnchorAssemblyAttribute"/> rule will only look for concrete types in the anchor Assembly.</param>        /// <returns>An instance of type <paramref name="type"/> if possible, null if unable to construct one.</returns>
        /// <returns>An instance of <paramref name="type"/></returns>
        protected static object InstanceFromConstructorRules(Type type, IEnumerable<IActivateAnythingRule> rules, IEnumerable<Type> typesWaitingToBeBuilt, object searchAnchor)
        {
            var constructor = ChooseConstructor(type, rules.OfType<IActivateAnythingChooseConstructorRule>() , typesWaitingToBeBuilt, searchAnchor);

            if (constructor == null || constructor.GetParameters().Length == 0)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                var pars = constructor.GetParameters()
                    .Select(p => Of(p.ParameterType, rules, typesWaitingToBeBuilt, searchAnchor))
                    .ToArray();
                return Activator.CreateInstance(type, pars);
            }
        }

        /// <summary>
        /// Apply the <paramref name="chooseConstructorRules"/> to choose a constructor for <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type for which a constructor is to be found</param>
        /// <param name="chooseConstructorRules">The rules for choosing a constructor. For instance,
        /// the <see cref="ChooseConstructorWithMostParametersAttribute"/> will choose a constructor with the most 
        /// parameters.
        /// The rule <see cref="ChooseConstructorWithFewestParametersAttribute"/> will always be added to the end of the
        /// <param name="chooseConstructorRules"> as the last fallback rule.
        /// </param>
        /// <param name="typesWaitingToBeBuilt">The 'stack' of types we are trying to build grows as instantiating a type 
        /// recursively requires the instantion of its constructor dependencies.
        /// This parameter is for the benefit of recursive rules whose strategy may vary depending on what we're trying to build.
        /// </param>
        /// <param name="searchAnchor">The <see cref="Type"/> and especially the <see cref="Type.Assembly"/> of <c>searchAnchor</c> may
        /// be used by some rules as a reference point—whether as a starting point or as a limit—to their 
        /// search. For instance, the <see cref="FindInAnchorAssemblyAttribute"/> rule will only look for concrete types in the anchor Assembly.</param>        /// <returns>An instance of type <paramref name="type"/> if possible, null if unable to construct one.</returns>
        /// <returns>An <see cref="ConstructorInfo"/> for type <paramref name="type"/></returns>
        protected internal static ConstructorInfo ChooseConstructor(Type type, IEnumerable<IActivateAnythingChooseConstructorRule> chooseConstructorRules, IEnumerable<Type> typesWaitingToBeBuilt, object searchAnchor)
        {
            return chooseConstructorRules
                .Union(new[] {new ChooseConstructorWithFewestParametersAttribute()})
                .Select(r => r.ChooseConstructor(type, typesWaitingToBeBuilt, searchAnchor))
                .FirstOrDefault();
        }

        /// <summary>
        /// Identical to <see cref="ActivateAnythingDefaultRulesAttribute.AllDefaultRules"/>
        /// </summary>
        public static readonly IEnumerable<IActivateAnythingRule> DefaultRules = ActivateAnythingDefaultRulesAttribute.AllDefaultRules;
    }
}