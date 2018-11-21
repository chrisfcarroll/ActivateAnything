using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

#if NET45
#elif NET40
#endif


namespace ActivateAnything
{
    /// <summary>
    ///     A class for creating instances of a <see cref="Type" />, whether or not the Type is
    ///     concrete, whether or not the Type has constructor dependencies, whether or not any suitable
    ///     concrete Subtype is currently loaded or known.
    ///     The attempts to find and instantiate suitable Types is driven by <see cref="Rules" /> and
    ///     <see cref="SearchAnchor" />.
    ///     There are three kinds of <see cref="IActivateAnythingRule" /> Rule.
    ///     <list type="bullet">
    ///         <item>
    ///             <see cref="IActivateInstanceRule" /> provides an immediate source of a concrete
    ///             type. For instance, the <see cref="CreateFromFactoryMethodAttribute" /> rule.
    ///         </item>
    ///         <item>
    ///             <see cref="IFindTypeRule" /> provides rules for where to look for candidate
    ///             concrete subTypes of an abstract type.
    ///         </item>
    ///         <item>
    ///             <see cref="IChooseConstructorRule" /> rules for how to choose between constructors when
    ///             the <see cref="IFindTypeRule" />s have found a concrete <c>Type</c> to instantiate.
    ///         </item>
    ///     </list>
    /// </summary>
    public partial class AnythingActivator
    {
        /// <summary>If the attempt to construct an instance recurses to this depth, recursion will stop
        /// by returning (at the <see cref="RecursionLimit"/>th level) the result of evaluating
        /// <see cref="RecursionLimitReturnFunc"/> 
        /// </summary>
        public int RecursionLimit { get; set; } = 99;

        /// <summary>The function used to create an instance if the recursion depth of <see cref="New"/> reaches
        /// <see cref="RecursionLimit"/>. Defaults to returning <c>default(T)</c> where typeof(T) is the
        /// <see cref="Type"/> being constructed at the level of the recursion limit.
        /// If you prefer that reaching the recursion limit should cause an Exception, then set this to throw.
        /// </summary>
        public Func<Type, IEnumerable<Type>, object, object> RecursionLimitReturnFunc = (type, types, anchor) => type.DefaultValue();

        /// <summary>
        ///     Create an <see cref="AnythingActivator" /> with the given <see cref="Rules" /> and no
        ///     <see cref="SearchAnchor" />.
        /// </summary>
        /// <param name="rules">The <see cref="Rules" /></param>
        public AnythingActivator(IEnumerable<IActivateAnythingRule> rules = null) { Rules = rules?.ToArray() ?? DefaultRules; }

        /// <summary>
        ///     Create an <see cref="AnythingActivator" /> with the given <see cref="Rules" /> and no
        ///     <see cref="SearchAnchor" />.
        /// </summary>
        /// <param name="rules">The <see cref="Rules" /></param>
        public AnythingActivator(params IActivateAnythingRule[] rules) { Rules = rules ?? DefaultRules; }

        /// <summary>
        ///     Create an <see cref="AnythingActivator" /> with the given <see cref="Rules" /> and <see cref="SearchAnchor" />.
        /// </summary>
        /// <param name="searchAnchor">The <see cref="SearchAnchor" /> to use.</param>
        /// <param name="rules">The <see cref="Rules" /></param>
        public AnythingActivator(object searchAnchor, IEnumerable<IActivateAnythingRule> rules = null)
        {
            SearchAnchor = searchAnchor;
            Rules = rules?.ToArray() ?? DefaultRules;
        }

        /// <summary>
        ///     Create an <see cref="AnythingActivator" /> with the given <see cref="Rules" /> and <see cref="SearchAnchor" />.
        /// </summary>
        /// <param name="searchAnchor">The <see cref="SearchAnchor" /> to use.</param>
        /// <param name="rules">The <see cref="Rules" /></param>
        public AnythingActivator(object searchAnchor, params IActivateAnythingRule[] rules)
        {
            SearchAnchor = searchAnchor;
            Rules = rules;
        }

        /// <summary>
        ///     Create an <see cref="AnythingActivator" /> using <paramref name="searchAnchorAndRuleProvider" /> as
        ///     the <see cref="SearchAnchor" /> and also as the source <see cref="IActivateAnythingRule" />s.
        ///     Rules are obtained from the <see cref="Type.CustomAttributes" /> of
        ///     <paramref name="searchAnchorAndRuleProvider" />.
        /// 
        ///     <seealso cref="TypeExtensions.GetActivateAnythingRuleAttributes" />
        /// </summary>
        /// <param name="searchAnchorAndRuleProvider">
        ///     will be used as the <see cref="SearchAnchor" /> and as the sources of
        ///     <see cref="Rules" />
        /// </param>
        public AnythingActivator(object searchAnchorAndRuleProvider)
        {
            SearchAnchor = searchAnchorAndRuleProvider;
            Rules = (IReadOnlyCollection<IActivateAnythingRule>)
                searchAnchorAndRuleProvider.GetType().GetActivateAnythingRuleAttributes();
        }

        /// <summary>
        ///     Each <see cref="IActivateAnythingRule" /> guides the choices needed to activate an instance of a type,
        ///     and, transitively, the dependencies, if any, of the type.
        ///     There are three kinds of <see cref="IActivateAnythingRule" /> Rule.
        ///     <list type="bullet">
        ///         <item>
        ///             <see cref="IActivateInstanceRule" /> provides an immediate source of a concrete
        ///             type. For instance, the <see cref="CreateFromFactoryMethodAttribute" /> rule.
        ///         </item>
        ///         <item>
        ///             <see cref="IFindTypeRule" /> provides rules for where to look for candidate
        ///             concrete subTypes of an abstract type.
        ///         </item>
        ///         <item>
        ///             <see cref="IChooseConstructorRule" /> rules for how to choose between constructors when
        ///             the <see cref="IFindTypeRule" />s have found a concrete <c>Type</c> to instantiate.
        ///         </item>
        ///     </list>
        /// </summary>
        public IReadOnlyCollection<IActivateAnythingRule> Rules { get; }


        /// <summary>
        ///     An object used by some <see cref="Rules" />, especially, <see cref="IFindTypeRule" />
        ///     rules, as a reference point—whether as a starting point or as a limit—to their search. For instance, the
        ///     <see cref="FindInAnchorAssemblyAttribute" /> rule will only look for concrete types in
        ///     <c>SearchAnchor.GetType().Assembly</c>, and <see cref="FindInAssembliesReferencedByAnchorAssembly" /> rule will
        ///     look in
        ///     each <see cref="Assembly" /> referenced by <c>SearchAnchor.GetType().Assembly</c>.
        /// </summary>
        public object SearchAnchor { get; }

        /// <summary>After a call to <see cref="New"/>, this will contain a list of Exceptions, if any,
        /// generated during the operation, keyed on the stack of <c>Type</c>s waiting to be built at
        /// the point the <c>Exception</c> was raised.
        /// </summary>
        public List<KeyValuePair<ActivationInfo, Exception>> LastErrorList { get; private set; } = new List<KeyValuePair<ActivationInfo, Exception>>();

        /// <summary>After a call to <see cref="New"/>, this will contain a list of Exceptions, if any,
        /// generated during the operation, keyed on the stack of <c>Type</c>s waiting to be built at
        /// the point the <c>Exception</c> was raised.
        /// </summary>
        public List<ActivationInfo> LastActivationTree { get; private set; } = new List<ActivationInfo>();

        /// <summary>
        ///     Creates an instance of something assignable to <typeparamref name="T" /> using <see cref="Rules" />
        ///     and <see cref="SearchAnchor" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>An instance of type <typeparamref name="T" /> if possible, <c>default(T)</c> if unable to construct one</returns>
        public T New<T>()
        {
            LastErrorList = new List<KeyValuePair<ActivationInfo, Exception>>();
            LastActivationTree= new List<ActivationInfo>();
            return (T) New(typeof(T), null);
        }

        /// <summary>
        ///     Creates an instance of something assignable to <paramref name="type" /> using <see cref="Rules" />
        ///     and <see cref="SearchAnchor" />
        /// </summary>
        /// <param name="type">type</param>
        /// <returns>An instance of type <paramref name="type"/></returns>
        public object New(Type type)
        {
            LastErrorList = new List<KeyValuePair<ActivationInfo, Exception>>();
            LastActivationTree= new List<ActivationInfo>();
            return New(type, null);
        }

        /// <summary>
        ///     Creates an instance of something assignable to <paramref name="type" /> using <see cref="Rules" />
        ///     and <see cref="SearchAnchor" />
        /// </summary>
        /// <param name="type">The type of which a concrete instance is wanted.</param>
        /// <param name="typesWaitingToBeBuilt">
        ///     The 'stack' of types we are trying to build grows as instantiating a type
        ///     recursively requires the instantion of its constructor dependencies.
        ///     This parameter is for the benefit of recursive rules whose strategy may vary depending on what we're trying
        ///     to build.
        /// </param>
        /// <returns>An instance of type <paramref name="type" /> if possible, <c>default(T)</c> if unable to construct one.</returns>
        object New(Type type, IEnumerable<Type> typesWaitingToBeBuilt)
        {
            if (typesWaitingToBeBuilt?.Count() >= RecursionLimit){
                return RecursionLimitReturnFunc(type, typesWaitingToBeBuilt,SearchAnchor);
            }

            typesWaitingToBeBuilt = (typesWaitingToBeBuilt ?? new List<Type>()).Union(new[] {type});

            var customRuleResult = Rules.OfType<IActivateInstanceRule>()
                .Select(r => r.CreateInstance(type, typesWaitingToBeBuilt, SearchAnchor))
                .FirstOrDefault();

            var ainfo = new ActivationInfo {TypeStack = typesWaitingToBeBuilt};
            try
            {
                if (customRuleResult != null)
                {
                    LastActivationTree.Add(ainfo=ActivationInfo.InstanceRule(typesWaitingToBeBuilt));
                    return customRuleResult;
                }
                else if (type.IsAbstract || type.IsInterface)
                {
                    ainfo = new ActivationInfo {How = "type.IsAbstract || type.IsInterface", TypeStack = typesWaitingToBeBuilt};
                    return New(
                        TypeFinder.FindConcreteTypeAssignableTo(type, Rules, typesWaitingToBeBuilt, SearchAnchor),
                        typesWaitingToBeBuilt);
                }
                else if (type == typeof(string))
                {
                    LastActivationTree.Add(ainfo=ActivationInfo.ValueType(typesWaitingToBeBuilt));
                    return "for"+typesWaitingToBeBuilt.Reverse().Skip(1).FirstOrDefault();
                }
                else if (type.IsValueType)
                {
                    LastActivationTree.Add(ainfo=ActivationInfo.ValueType(typesWaitingToBeBuilt));
                    return Activator.CreateInstance(type);
                }
                else
                    ainfo = new ActivationInfo {How = "InstanceFromConstructorRules", TypeStack = typesWaitingToBeBuilt};
                    return InstanceFromConstructorRules(type, Rules, typesWaitingToBeBuilt, SearchAnchor);
            } 
            catch (Exception e)
            {
                LastErrorList?.Add(new KeyValuePair<ActivationInfo, Exception>(ainfo, e));
                return null;
            }
        }

        /// <summary>
        ///     Use <see cref="Activator.CreateInstance(System.Type)" /> to activate an Instance of the concrete type
        ///     <paramref name="type" />. Use the <see cref="IChooseConstructorRule" /> to choose a constructor.
        ///     If the chosen constructor has dependencies, then recursively use <see cref="New" /> to create them.
        /// </summary>
        /// <param name="type">This should be a concrete type, otherwise activation will fail.</param>
        /// <param name="rules">
        ///     <see cref="IActivateAnythingRule" /> rules are of three kinds:
        ///     <list type="bullet">
        ///         <item>
        ///             <see cref="IActivateInstanceRule" /> provides an immediate source of a concrete
        ///             type. For instance, the <see cref="CreateFromFactoryMethodAttribute" /> rule.
        ///         </item>
        ///         <item>
        ///             <see cref="IFindTypeRule" /> provides rules for where to look for candidate
        ///             concrete subTypes of <paramref name="type"/>.
        ///         </item>
        ///         <item>
        ///             <see cref="IChooseConstructorRule" /> rules for how to choose between constructors when
        ///             the <see cref="IFindTypeRule" />s have found a <c>Type</c> to instantiate.
        ///         </item>
        ///     </list>
        ///     If<paramref name="rules" /> is null, the <see cref="DefaultRules" /> will be used.
        /// </param>
        /// <param name="typesWaitingToBeBuilt">
        ///     The 'stack' of types we are trying to build grows as instantiating a type
        ///     recursively requires the instantion of its constructor dependencies.
        ///     This parameter is for the benefit of recursive rules whose strategy may vary depending on what we're trying to
        ///     build.
        /// </param>
        /// <param name="searchAnchor">
        ///     The <see cref="Type" /> and especially the <see cref="Type.Assembly" /> of
        ///     <c>searchAnchor</c> may be used by some rules as a reference point—whether as a starting point or as a limit—to
        ///     their search. For instance, the <see cref="FindInAnchorAssemblyAttribute" /> rule will only look for concrete
        ///     types in the anchor Assembly.
        /// </param>
        /// <returns>An instance of type <paramref name="type" /> if possible; default(Type) if unable to construct one.</returns>
        protected object InstanceFromConstructorRules(
            Type type,
            IEnumerable<IActivateAnythingRule> rules,
            IEnumerable<Type> typesWaitingToBeBuilt,
            object searchAnchor)
        {
            var constructor = ChooseConstructor(type,
                rules.OfType<IChooseConstructorRule>(),
                typesWaitingToBeBuilt,
                searchAnchor);

            if (constructor == null)
            {
                var instance = Activator.CreateInstance(type);
                LastActivationTree.Add(ActivationInfo.NoConstructor(typesWaitingToBeBuilt));
                return instance;
            }
            else if (constructor.GetParameters().Length == 0 && constructor.IsPublic)
            {
                var instance = Activator.CreateInstance(type);
                LastActivationTree.Add(ActivationInfo.Constructed(typesWaitingToBeBuilt,constructor));
                return instance;
            }
            else if (constructor.GetParameters().Length == 0 && !constructor.IsPublic)
            {
                var instance = Activator.CreateInstance(type,true);
                LastActivationTree.Add(ActivationInfo.Constructed(typesWaitingToBeBuilt,constructor));
                return instance;
            }
            else
            {
                var bindingFlags = constructor.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic;
                bindingFlags |= BindingFlags.Instance;
                var pars = constructor.GetParameters()
                    .Select(p => 
                            typesWaitingToBeBuilt.Contains(p.ParameterType) && p.IsOptional
                                ? p.ParameterType.DefaultValue()
                                : New(p.ParameterType, typesWaitingToBeBuilt)
                        )
                    .ToArray();
                var instance = constructor.Invoke(bindingFlags, null, pars, CultureInfo.CurrentCulture);
                LastActivationTree.Add(ActivationInfo.Constructed(typesWaitingToBeBuilt,constructor, pars));
                return instance;
            }
        }

        /// <summary>
        ///     Apply the <paramref name="chooseConstructorRules" /> to choose a constructor for <paramref name="type" />.
        /// </summary>
        /// <param name="type">The type for which a constructor is to be found</param>
        /// <param name="chooseConstructorRules">
        ///     The rules for choosing a constructor. For instance,
        ///     the <see cref="ChooseConstructorWithMostParametersAttribute" /> will choose a constructor with the most
        ///     parameters.
        ///     The rule <see cref="ChooseConstructorWithFewestParametersAttribute" /> will always be added to the end of the
        ///     <paramref name="chooseConstructorRules" /> as the last fallback rule.
        /// </param>
        /// <param name="typesWaitingToBeBuilt">
        ///     The 'stack' of types we are trying to build grows as instantiating a type
        ///     recursively requires the instantion of its constructor dependencies.
        ///     This parameter is for the benefit of recursive rules whose strategy may vary depending on what we're trying to
        ///     build.
        /// </param>
        /// <param name="searchAnchor">
        ///     The <see cref="Type" /> and especially the <see cref="Type.Assembly" /> of
        ///     <c>searchAnchor</c> may be used by some rules as a reference point—whether as a starting point or as a limit—to
        ///     their search. For instance, the <see cref="FindInAnchorAssemblyAttribute" /> rule will only look for concrete types
        ///     in the anchor Assembly.
        /// </param>
        /// <returns>An instance of type <paramref name="type" /> if possible, default(Type) if unable to construct one.</returns>
        /// <returns>An <see cref="ConstructorInfo" /> for type <paramref name="type" /></returns>
        protected internal ConstructorInfo ChooseConstructor(
            Type type,
            IEnumerable<IChooseConstructorRule> chooseConstructorRules,
            IEnumerable<Type> typesWaitingToBeBuilt,
            object searchAnchor)
        {
            var possibleConstructors = chooseConstructorRules
                .Union(new[] {new ChooseConstructorWithFewestParametersAttribute{PreferPublic = false}})
                .Select(r => r.ChooseConstructor(type, typesWaitingToBeBuilt, searchAnchor));
            var chosenConstructor = possibleConstructors.FirstOrDefault(r=>r!=null);
            return chosenConstructor;
        }
#pragma warning restore 1591
    }
}