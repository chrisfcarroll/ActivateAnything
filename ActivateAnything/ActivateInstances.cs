using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ActivateAnything
{
    /// <summary>An <see cref="IActivateInstanceRule" /> which returns previously created objects</summary>
    public class ActivateInstances : IActivateInstanceRule
    {
        /// <summary>
        ///     If this rule is asked for a type which can be instantiated by one of <paramref name="instances" />
        ///     then that object will be returned.
        /// </summary>
        /// <param name="instances"></param>
        public ActivateInstances(params object[] instances) { Instances = instances; }

        /// <summary>
        ///     If this is true, then this <see cref="ActivateInstances" /> instance will not
        ///     only fulfill requests for an object, but also requests for a <c>Func&lt;T&gt;</c> if one of
        ///     <see cref="Instances" /> is assignable to <c>T</c>
        /// </summary>
        public bool AlsoReturnFuncOfType { get; set; } = true;

        /// <summary>
        ///     The instances from which this rule will return one if <see cref="CreateInstance" /> is called
        ///     with a <see cref="Type" /> that is assignable from one of these instances.
        /// </summary>
        public object[] Instances { get; }

        /// <inheritdoc />
        public object CreateInstance(Type type, IEnumerable<Type> typesWaitingToBeBuilt, object searchAnchor = null)
        {
            return InstanceOfType(type) ?? CreateFactory(type) ?? null;
        }

        object InstanceOfType(Type type) { return Instances.FirstOrDefault(type.IsInstanceOfType); }

        object CreateFactory(Type type)
        {
            if (AlsoReturnFuncOfType
                && type.IsConstructedGenericType
                && type.GetGenericTypeDefinition() == typeof(Func<>)
                && type.GetGenericArguments().Length == 1)
            {
                var targetType = type.GetGenericArguments()[0];
                var instance = Instances.FirstOrDefault(targetType.IsInstanceOfType);
                if (instance != null)
                {
                    var funcTyped = typeof(Func<>).MakeGenericType(targetType);
                    var wrappedType = typeof(TypeWrapper<>).MakeGenericType(targetType);
                    var constructor = wrappedType.GetConstructor(new[] {typeof(object)});
                    var wrapper = constructor.Invoke(new[] {instance});
                    var instanceMethod = wrapper.GetType().GetMethod("Instance", BindingFlags.Public | BindingFlags.Instance);
                    return Delegate.CreateDelegate(funcTyped, wrapper, instanceMethod);
                }
            }

            return null;
        }

        class TypeWrapper<T>
        {
            readonly T instance;
            public TypeWrapper(object instance) { this.instance = (T) instance; }

            public T Instance() { return instance; }
        }
    }
}
