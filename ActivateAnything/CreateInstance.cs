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
        /// of <paramref name="anchorAssemblyType"/>.
        /// </summary>
        /// <param name="anchorAndRuleProvider">Pass in an object whose
        /// <list type="bullet">
        /// <item><description><see cref="Type.Attributes"/> will the source of the <see cref="IActivateAnythingRule"/> search 
        /// rules. If the object has no <see cref="IActivateAnythingRule"/> attributes, then the <see cref="DefaultRules"/> will 
        /// be used.</description></item>
        /// <item><description><see cref="Type.Assembly"/> will be used as the search anchor. Some rules use the anchor 
        /// as a reference point—whether as a starting point or as a limit—to their search. For instance, the 
        /// <see cref="FindOnlyInAnchorAssemblyAttribute"/> rule will only look for concrete types in the anchor Assembly.
        /// </description></item>
        /// </list>
        /// </param>
        /// <returns>An instance of type <typeparamref name="T"/></returns>
        public static T Of<T>(object anchorAndRuleProvider)
        {
            return (T)Of(typeof(T), anchorAndRuleProvider.GetType().GetActivateAnythingRulesFromAttributes(), typesWaitingToBeBuilt:null, anchorAssemblyType: anchorAndRuleProvider);
        }

        /// <summary>
        /// Creates an instance of something assignable to <typeparamref name="T"/>. Does so using the given <paramref name="rules"/>.
        /// Note that since this overload doesn't take a <code>anchorAndRuleProvider</code> parameter, rules such as 
        /// <see cref="FindOnlyInAnchorAssemblyAttribute"/> which depend on that parameter will not run.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rules"><see cref="IActivateAnythingRule"/>s include rules for where to look for instantiable types and which constructor to use
        /// If <paramref name="rules"/> is null, the <see cref="DefaultRules"/> will be used.
        /// </param>
        /// <returns>An instance of type <typeparamref name="T"/></returns>
        public static T Of<T>(IEnumerable<IActivateAnythingRule> rules= null)
        {
            return (T)Of(typeof(T), rules, null);
        }

        /// <summary>
        /// Creates an instance of something assignable to <typeparamref name="T"/>. Does so using the given  <paramref name="rules"/>.
        /// </summary>
        /// <param name="rules"><see cref="IActivateAnythingRule"/> rules include rules for where to look for types to use as constructor
        /// parameters</param>
        /// <param name="anchorAssemblyType">typically, you will pass in the TestFixture which is trying to build a concrete class. 
        /// The behaviour of some rules for finding concrete types—for instance <see cref="FindOnlyInAnchorAssemblyAttribute"/>—will be
        /// driven by the the type (or more likely the assembly) of this object.</param>
        /// <returns>An instance of type <typeparamref name="T"/></returns>
        public static T Of<T>(IEnumerable<IActivateAnythingRule> rules, object anchorAssemblyType)
        {
            return (T) Of(typeof (T), rules, typesWaitingToBeBuilt:null, anchorAssemblyType: anchorAssemblyType);
        }

        /// <summary>
        /// Creates an instance of something assignable to <paramref name="type"/>. Does so using the given <paramref name="rules"/>.
        /// Note that since this overload doesn't take a <code>anchorAndRuleProvider</code> parameter, rules such as 
        /// <see cref="FindOnlyInAnchorAssemblyAttribute"/> which depend on that parameter will not run.
        /// </summary>
        /// <param name="type">The type of which a concrete instance is wanted.</param>
        /// <param name="rules"><see cref="IActivateAnythingRule"/>s include rules for where to look for instantiable types and which constructor to use
        /// If <paramref name="rules"/> is null, the <see cref="DefaultRules"/> will be used.
        /// </param>
        /// <param name="typesWaitingToBeBuilt">The 'stack' of types we are trying to build grows as instantiating a type 
        /// recursively requires the instantion of its constructor dependencies.
        /// This parameter is for the benefit of build rules whose strategy may vary depending an what we're trying to build.
        /// </param>
        /// <param name="anchorAssemblyType">The object which raised the original request to instantiate something. 
        /// This parameter is for the benefit of build rules whose strategy may vary depending on what we're trying to build. 
        /// For instance, <see cref="FindOnlyInAnchorAssemblyAttribute"/> takes <paramref name="anchorAssemblyType"/> to be the TestFixture
        /// </param>
        /// <returns>An instance of type <paramref name="type"/> if possible, null if unable to construct one.</returns>
        public static object Of(Type type, IEnumerable<IActivateAnythingRule> rules=null, IEnumerable<Type> typesWaitingToBeBuilt = null, object anchorAssemblyType = null)
        {
            rules = rules ?? DefaultRules;
            typesWaitingToBeBuilt = (typesWaitingToBeBuilt ?? new List<Type>()).Union(new[] { type });

            var customRuleResult=rules.OfType<IActivateAnythingCreateInstanceRule>()
                .Select(r => r.CreateInstance(type, typesWaitingToBeBuilt, anchorAssemblyType))
                .FirstOrDefault();

            if(customRuleResult!=null)
            {
                return customRuleResult;
            }
            else if (type.IsAbstract || type.IsInterface)
            {
                return Of(TypeFinder.FindConcreteTypeAssignableTo(type, rules, typesWaitingToBeBuilt, anchorAssemblyType), rules, typesWaitingToBeBuilt, anchorAssemblyType);
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
                return InstanceFromConstructorRules(type, rules, typesWaitingToBeBuilt, anchorAssemblyType);
            }
        }

        protected static object InstanceFromConstructorRules(Type type, IEnumerable<IActivateAnythingRule> rules, IEnumerable<Type> typesWaitingToBeBuilt, object anchorAssemblyType)
        {
            var constructor = ChooseConstructor(type, rules.OfType<IActivateAnythingChooseConstructorRule>() , typesWaitingToBeBuilt, anchorAssemblyType);

            if (constructor == null || constructor.GetParameters().Length == 0)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                var pars = constructor.GetParameters()
                    .Select(p => Of(p.ParameterType, rules, typesWaitingToBeBuilt, anchorAssemblyType))
                    .ToArray();
                return Activator.CreateInstance(type, pars);
            }
        }

        protected internal static ConstructorInfo ChooseConstructor(Type type, IEnumerable<IActivateAnythingChooseConstructorRule> rules, IEnumerable<Type> typesWaitingToBeBuilt, object anchorAssemblyType)
        {
            return rules
                .Union(new[] {new ChooseConstructorWithFewestParametersAttribute()})
                .Select(r => r.ChooseConstructor(type, typesWaitingToBeBuilt, anchorAssemblyType))
                .FirstOrDefault();
        }

        /// <summary>
        /// Identical to <see cref="ActivateAnythingDefaultRulesAttribute.AllDefaultRules"/>
        /// </summary>
        public static readonly IEnumerable<IActivateAnythingRule> DefaultRules = ActivateAnythingDefaultRulesAttribute.AllDefaultRules;
    }
}