using System;
using System.Collections.Generic;

namespace ActivateAnything
{
    /// <summary>An <see cref="IActivateInstanceRule"/> which returns a previously created object
    /// when asked for a <typeparamref name="T"/></summary>
    public class ActivateForType<T> : IActivateInstanceRule
    {
        readonly T value;

        /// <summary>
        /// If this rule is asked for an instance of type <typeparamref name="T"/> then <paramref name="value"/>
        /// will be returned.
        /// </summary>
        /// <param name="value"></param>
        public ActivateForType(T value) { this.value = value; }

        /// <inheritdoc />
        public object CreateInstance(Type type, IEnumerable<Type> typesWaitingToBeBuilt, object searchAnchor = null)
        {
            return type.IsAssignableFrom(typeof(T)) 
                ? (object) value 
                : null;
        }
    }
}