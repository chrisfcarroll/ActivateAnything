using System;
using System.Collections.Generic;
using System.Linq;

namespace ActivateAnything
{
    /// <summary>An <see cref="IActivateInstanceRule"/> which returns previously created objects</summary>
    public class ActivateInstances : IActivateInstanceRule
    {
        /// <summary>If this is true, then this <see cref="ActivateInstances"/> instance will not 
        /// only fulfill requests for an object, but also requests for a <c>Func<T></c> if one of
        /// <see cref="Instances"/> is assignable to <c>T</c>
        /// </summary>
        bool AlsoReturnFuncType { get; set; } = false;
        
        /// <summary>The instances from which this rule will return one if <see cref="CreateInstance"/> is called
        /// with a <see cref="Type"/> that is assignable from one of these instances.</summary>
        public object[] Instances { get; }

        /// <summary>
        /// If this rule is asked for a type which can be instantiated by one of <paramref name="instances"/>
        /// then that object will be returned.
        /// </summary>
        /// <param name="instances"></param>
        public ActivateInstances(params object[] instances) { this.Instances = instances; }
        
        /// <inheritdoc />
        public object CreateInstance(Type type, IEnumerable<Type> typesWaitingToBeBuilt, object searchAnchor = null)
        {
            return
                Instances.FirstOrDefault(type.IsInstanceOfType)
                    ?? CreateFactory(type);
        }
        object CreateFactory(Type type)
        {
            if (AlsoReturnFuncType 
             && type.IsConstructedGenericType 
             && type.GetGenericTypeDefinition() == functype 
             && type.GetGenericArguments().Length==1)
            {
                var targetType = type.GetGenericArguments()[0];
                return Instances
                          .Where(targetType.IsInstanceOfType)
                          .Select<object,Func<object>>(i => (() => i))
                          .FirstOrDefault();
            }
            return null;
        }

        static readonly Type functype = typeof(Func<>);
    }
}