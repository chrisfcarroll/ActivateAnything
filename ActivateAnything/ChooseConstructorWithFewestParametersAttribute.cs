using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ActivateAnything
{
    /// <summary>Choose the constructor with the most parameters, after respecting
    /// <list type="number">
    ///     <item>the <see cref="PreferPublic"/> setting if it is true.</item>
    ///     <item>the <see cref="ChooseConstructorRuleAttribute.NoCircularDependencyRule"/></item>
    /// </list>
    /// </summary>
    public class ChooseConstructorWithFewestParametersAttribute : ChooseConstructorRuleAttribute
    {
        /// <summary>If true then any public constructor will be chosen in preference to any non-public constructor.
        /// Defaults to <c>true</c>.</summary>
        public bool PreferPublic { get; set; } = true;

        /// <inheritdoc />
        public override ConstructorInfo ChooseConstructor(
            Type type,
            IEnumerable<Type> typesWaitingToBeBuilt,
            object searchAnchor = null)
        {
            Func<ConstructorInfo, bool> noCircularDependency = c=>NoCircularDependencyRule(typesWaitingToBeBuilt,c);

            return type.GetConstructors(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance)
                .Where(noCircularDependency)
                .OrderByDescending(c => PreferPublic && c.IsPublic)
                .ThenBy(c => c.GetParameters().Length)
                .FirstOrDefault();
        }
    }
}