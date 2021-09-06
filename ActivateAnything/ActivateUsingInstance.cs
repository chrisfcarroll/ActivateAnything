using System;
using System.Collections.Generic;
using System.Reflection;

namespace ActivateAnything
{
    /// <summary>
    ///     An <see cref="IActivateInstanceRule" /> which returns a previously created object
    ///     when asked for a <typeparamref name="T" />
    /// The object is identified by Class and PropertyName so that this Rule can be used an Attribute. 
    /// </summary>
    public class ActivateUsingInstance : Attribute, IActivateInstanceRule
    {
        readonly Type requiredType;
        readonly object value;

        /// <summary>
        ///     If this rule is asked for an instance of type <typeparamref name="T" /> then <paramref name="value" />
        ///     will be returned.
        /// </summary>
        /// <param name="requiredType"> The type to match when <see cref="CreateInstance"/> is called.
        /// This rule will match if the type requested IsAssignableFrom <paramref name="requiredType"/> 
        /// </param>
        /// <param name="instanceSourceClass">The class to inspect by reflection for a Property that can
        /// be used to satisfy a request for a <paramref name="requiredType"/>
        /// </param>
        /// <param name="instancePropertyName">The Property to inspect by reflection to satisfy
        /// a request for a <paramref name="requiredType"/>
        /// </param>
        public ActivateUsingInstance(Type requiredType, Type instanceSourceClass, string instancePropertyName)
        {
            this.requiredType = requiredType;
            var property = instanceSourceClass
                           .GetProperty(instancePropertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            if (property != null)
            {
                this.value = property.GetValue(null);
            }
            else
            {
                var field = instanceSourceClass
                               .GetField(
                                         instancePropertyName,
                                         BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                this.value = field?.GetValue(null);
            }
        }

        /// <inheritdoc />
        public object CreateInstance(Type type, IEnumerable<Type> typesWaitingToBeBuilt, object searchAnchor = null)
        {
            return type.IsAssignableFrom(requiredType)
                                   ? value
                                   : null;
        }
    }
}
