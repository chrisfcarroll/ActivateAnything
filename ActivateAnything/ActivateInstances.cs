using System;
using System.Collections.Generic;
using System.Linq;

namespace ActivateAnything
{
    /// <summary>An <see cref="IActivateInstanceRule"/> which returns previously created objects</summary>
    public class ActivateInstances : IActivateInstanceRule
    {
        readonly object[] objects;

        /// <summary>
        /// If this rule is asked for a type which can be instantiated by one of <paramref name="objects"/>
        /// then that object will be returned.
        /// </summary>
        /// <param name="objects"></param>
        public ActivateInstances(params object[] objects) { this.objects = objects; }
        
        /// <inheritdoc />
        public object CreateInstance(Type type, IEnumerable<Type> typesWaitingToBeBuilt, object searchAnchor = null)
        {
            return objects.FirstOrDefault(type.IsInstanceOfType);
        }
    }
}