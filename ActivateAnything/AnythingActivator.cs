﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ActivateAnything
{
    /// <summary>A class for creating instances of a <see cref="Type"/>, whether or not the Type is
    /// concrete, whether or not the Type has constructor dependencies, whether or not any suitable
    /// concrete Subtype is currently loaded or known.
    /// The attempts to find and instantiate suitable Types is driven by <see cref="Rules"/> and <see cref="SearchAnchor"/>.
    /// 
    /// There are three kinds of <see cref="IActivateAnythingRule"/> Rule.
    /// <list type="bullet">
    /// <item><see cref="IActivateAnythingCreateInstanceRule"/> provides an immediate source of a concrete
    /// type. For instance, the <see cref="CreateFromFactoryMethodAttribute"/> rule.</item>
    /// 
    /// <item><see cref="IActivateAnythingFindConcreteTypeRule"/> provides rules for where to look for candidate 
    /// concrete subTypes of an abstract type.</item>
    /// 
    /// <item><see cref="IActivateAnythingChooseConstructorRule"/> rules for how to choose between constructors when
    /// the <see cref="IActivateAnythingFindConcreteTypeRule"/>s have found a concrete <c>Type</c> to instantiate.</item>
    /// </list>
    /// </summary>
    public class AnythingActivator
    {
        /// <summary>Identical to <see cref="ActivateAnythingDefaultRulesAttribute.AllDefaultRules"/> </summary>
        public static IReadOnlyCollection<IActivateAnythingRule> 
                        DefaultRules => ActivateAnythingDefaultRulesAttribute.AllDefaultRules;

        /// <summary>An instance of <see cref="AnythingActivator"/> which uses <see cref="DefaultRules"/>
        /// and has no searchAnchor.</summary>
        public static AnythingActivator Instance= new AnythingActivator(DefaultRules);

        /// <summary>An instance of <see cref="AnythingActivator"/> which uses <see cref="DefaultRules"/> for its
        /// <see cref="Rules"/> and uses <paramref name="searchAnchor"/> for its <see cref="SearchAnchor"/></summary>
        /// <param name="searchAnchor">The <see cref="SearchAnchor"/> to use.</param>
        public static AnythingActivator FromAnchor(object searchAnchor) => new AnythingActivator(searchAnchor, DefaultRules);

        /// <summary>Each <see cref="IActivateAnythingRule"/> guides the choices needed to activate an instance of a type,
        /// and, transitively, the dependencies, if any, of the type.
        /// There are three kinds of <see cref="IActivateAnythingRule"/> Rule.
        /// 
        /// <list type="bullet">
        /// <item><see cref="IActivateAnythingCreateInstanceRule"/> provides an immediate source of a concrete
        /// type. For instance, the <see cref="CreateFromFactoryMethodAttribute"/> rule.</item>
        /// 
        /// <item><see cref="IActivateAnythingFindConcreteTypeRule"/> provides rules for where to look for candidate 
        /// concrete subTypes of an abstract type.</item>
        /// 
        /// <item><see cref="IActivateAnythingChooseConstructorRule"/> rules for how to choose between constructors when
        /// the <see cref="IActivateAnythingFindConcreteTypeRule"/>s have found a concrete <c>Type</c> to instantiate.</item>
        /// </list>
        /// </summary>
        public IReadOnlyCollection<IActivateAnythingRule> Rules { get; }

        
        /// <summary>An object used by some <see cref="Rules"/>, especially, <see cref="IActivateAnythingFindConcreteTypeRule"/>
        /// rules, as a reference point—whether as a starting point or as a limit—to their search. For instance, the
        /// <see cref="FindInAnchorAssemblyAttribute"/> rule will only look for concrete types in
        /// <c>SearchAnchor.GetType().Assembly</c>, and <see cref="FindInAssembliesReferencedByAnchorAssembly"/> rule will look in
        /// each <see cref="Assembly"/> referenced by <c>SearchAnchor.GetType().Assembly</c>.
        /// </summary>
        public object SearchAnchor { get; }

        /// <summary>Create an <see cref="AnythingActivator"/> with the given <see cref="Rules"/> and no
        /// <see cref="SearchAnchor"/>.</summary>
        /// <param name="rules">The <see cref="Rules"/></param>
        public AnythingActivator(IEnumerable<IActivateAnythingRule> rules=null) 
                => Rules = rules?.ToArray() ?? DefaultRules;

        /// <summary>Create an <see cref="AnythingActivator"/> with the given <see cref="Rules"/> and no
        /// <see cref="SearchAnchor"/>.</summary>
        /// <param name="rules">The <see cref="Rules"/></param>
        public AnythingActivator(params IActivateAnythingRule[] rules) => Rules = rules ?? DefaultRules;

        /// <summary>Create an <see cref="AnythingActivator"/> with the given <see cref="Rules"/> and <see cref="SearchAnchor"/>.
        /// </summary>
        /// <param name="searchAnchor">The <see cref="SearchAnchor"/> to use.</param>
        /// <param name="rules">The <see cref="Rules"/></param>
        public AnythingActivator(object searchAnchor, IEnumerable<IActivateAnythingRule> rules = null)
        {
            SearchAnchor = searchAnchor;
            Rules =  rules?.ToArray() ?? DefaultRules;
        }

        /// <summary>Create an <see cref="AnythingActivator"/> with the given <see cref="Rules"/> and <see cref="SearchAnchor"/>.
        /// </summary>
        /// <param name="searchAnchor">The <see cref="SearchAnchor"/> to use.</param>
        /// <param name="rules">The <see cref="Rules"/></param>
        public AnythingActivator(object searchAnchor, params IActivateAnythingRule[] rules)
        {
            SearchAnchor = searchAnchor;
            Rules = rules;
        }

        /// <summary>Create an <see cref="AnythingActivator"/> using <paramref name="searchAnchorAndRuleProvider"/> as
        /// the <see cref="SearchAnchor"/> and also as the source <see cref="IActivateAnythingRule"/>s.
        /// Rules are obtained from the <see cref="Type.CustomAttributes"/> of <paramref name="searchAnchorAndRuleProvider"/>.
        /// <seealso cref="TypeExtensions.GetActivateAnythingRuleAttributes"/></summary>
        /// <param name="searchAnchorAndRuleProvider"> will be used as the <see cref="SearchAnchor"/> and as the sources of
        /// <see cref="Rules"/></param>
        public AnythingActivator(object searchAnchorAndRuleProvider)
        {
            SearchAnchor = searchAnchorAndRuleProvider;
            Rules = (IReadOnlyCollection<IActivateAnythingRule>) 
                        searchAnchorAndRuleProvider.GetType().GetActivateAnythingRuleAttributes();
        }

        /// <summary>Creates an instance of something assignable to <typeparamref name="T"/> using <see cref="Rules" />
        /// and <see cref="SearchAnchor"/></summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>An instance of type <typeparamref name="T"/> if possible, <c>default(T)</c> if unable to construct one</returns>
        public T Of<T>() => (T)Of(typeof(T), null);

        /// <summary>Creates an instance of something assignable to <paramref name="type"/> using <see cref="Rules" />
        /// and <see cref="SearchAnchor"/></summary>
        /// <param name="type">type</param>
        /// <returns>An instance of type <typeparamref name="T"/></returns>
        public object Of(Type type) => Of(type, null);


        /// <summary>Creates an instance of something assignable to <paramref name="type"/> using <see cref="Rules" />
        /// and <see cref="SearchAnchor" /></summary>
        /// <param name="type">The type of which a concrete instance is wanted.</param>
        /// <param name="typesWaitingToBeBuilt">The 'stack' of types we are trying to build grows as instantiating a type 
        /// recursively requires the instantion of its constructor dependencies.
        /// This parameter is for the benefit of recursive rules whose strategy may vary depending on what we're trying
        /// to build.</param>
        /// <returns>An instance of type <paramref name="type"/> if possible, <c>default(T)</c> if unable to construct one.</returns>
        object Of(Type type, IEnumerable<Type> typesWaitingToBeBuilt)
        {
            typesWaitingToBeBuilt = (typesWaitingToBeBuilt ?? new List<Type>()).Union(new[] { type });

            var customRuleResult=Rules.OfType<IActivateAnythingCreateInstanceRule>()
                .Select(r => r.CreateInstance(type, typesWaitingToBeBuilt, SearchAnchor))
                .FirstOrDefault();

            if(customRuleResult!=null)
            {
                return customRuleResult;
            }
            else if (type.IsAbstract || type.IsInterface)
            {
                return Of( TypeFinder.FindConcreteTypeAssignableTo(type, Rules, typesWaitingToBeBuilt, SearchAnchor), 
                           typesWaitingToBeBuilt);
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
                return InstanceFromConstructorRules(type, Rules, typesWaitingToBeBuilt, SearchAnchor);
            }
        }

        /// <summary>Use <see cref="Activator.CreateInstance(System.Type)"/> to activate an Instance of the concrete type
        /// <paramref name="type"/>. Use the <see cref="IActivateAnythingChooseConstructorRule"/> to choose a constructor.
        /// If the chosen constructor has dependencies, then recursively use <see cref="Of"/> to create them.
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
        /// This parameter is for the benefit of recursive rules whose strategy may vary depending on what we're trying to
        /// build.</param>
        /// <param name="searchAnchor">The <see cref="Type"/> and especially the <see cref="Type.Assembly"/> of
        /// <c>searchAnchor</c> may be used by some rules as a reference point—whether as a starting point or as a limit—to
        /// their search. For instance, the <see cref="FindInAnchorAssemblyAttribute"/> rule will only look for concrete
        /// types in the anchor Assembly.</param>
        /// <returns>An instance of type <paramref name="type"/> if possible, default(Type) if unable to construct one.</returns>
        /// <returns>An instance of <paramref name="type"/></returns>
        protected object InstanceFromConstructorRules(Type type, IEnumerable<IActivateAnythingRule> rules, 
                                                      IEnumerable<Type> typesWaitingToBeBuilt, object searchAnchor)
        {
            var constructor = ChooseConstructor(type, 
                                                rules.OfType<IActivateAnythingChooseConstructorRule>(), 
                                                typesWaitingToBeBuilt, 
                                                searchAnchor);

            if (constructor == null || constructor.GetParameters().Length == 0)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                var pars = constructor.GetParameters()
                    .Select(p => Of(p.ParameterType, typesWaitingToBeBuilt))
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
        /// <paramref name="chooseConstructorRules"/> as the last fallback rule.
        /// </param>
        /// <param name="typesWaitingToBeBuilt">The 'stack' of types we are trying to build grows as instantiating a type 
        /// recursively requires the instantion of its constructor dependencies.
        /// This parameter is for the benefit of recursive rules whose strategy may vary depending on what we're trying to build.
        /// </param>
        /// <param name="searchAnchor">The <see cref="Type"/> and especially the <see cref="Type.Assembly"/> of
        /// <c>searchAnchor</c> may be used by some rules as a reference point—whether as a starting point or as a limit—to
        /// their search. For instance, the <see cref="FindInAnchorAssemblyAttribute"/> rule will only look for concrete types
        /// in the anchor Assembly.</param>
        /// <returns>An instance of type <paramref name="type"/> if possible, default(Type) if unable to construct one.</returns>
        /// <returns>An <see cref="ConstructorInfo"/> for type <paramref name="type"/></returns>
        protected internal ConstructorInfo ChooseConstructor(
                                            Type type, 
                                            IEnumerable<IActivateAnythingChooseConstructorRule> chooseConstructorRules, 
                                            IEnumerable<Type> typesWaitingToBeBuilt, object searchAnchor)
        {
            return chooseConstructorRules
                .Union(new[] {new ChooseConstructorWithFewestParametersAttribute()})
                .Select(r => r.ChooseConstructor(type, typesWaitingToBeBuilt, searchAnchor))
                .FirstOrDefault();
        }
    }
}