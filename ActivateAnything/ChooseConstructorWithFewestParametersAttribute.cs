using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ActivateAnything
{
    /// <summary>Choose the constructor with the most parameters, after respecting the <see cref="PreferPublic"/> setting if it is true.</summary>
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
            return type.GetConstructors(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance)
                .OrderByDescending(c => PreferPublic && c.IsPublic)
                .ThenBy(c => c.GetParameters().Length)
                .FirstOrDefault();
        }
    }
}