using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ActivateAnything
{
    /// <summary>A rule which provides a factory method capable of creating an instance of a Type.</summary>
    public class CreateFromFactoryMethodAttribute : Attribute, IActivateAnythingCreateInstanceRule
    {
        readonly Type targetType;
        readonly Type factoryClass;
        readonly string factoryMethodName;
        readonly object[] args;

        /// <summary>
        /// Specifies a factory method which can create an instance of the <paramref name="targetType"/>
        /// </summary>
        /// <param name="targetType">The type, the creating of an instance of which should be done by a factory method.</param>
        /// <param name="factoryClass">The class where the factory method can be found. 
        /// If <paramref name="factoryMethodName"/> is not static then it must be possible to create an instance (for instance, 
        /// by calling a constructor with no parameters, or with parameters we can recursively create)</param>
        /// <param name="factoryMethodName">The factory method to call each time we try to create a <paramref name="targetType"/></param>
        /// <param name="args">any arguments needed for the factory method</param>
        public CreateFromFactoryMethodAttribute(Type targetType, Type factoryClass, string factoryMethodName, params object[] args)
        {
            this.targetType = targetType;
            this.factoryClass = factoryClass;
            this.factoryMethodName = factoryMethodName;
            this.args = args;
            if(factoryClass != null){ EnsureFactoryMethodElseThrow(factoryClass,null); }
        }
        /// <summary>
        /// Specifies a factory method which can create an instance of the <paramref name="targetType"/>.
        /// The method must exist on the <c>searchAnchor</c> object passed to <see cref="CreateInstance.Of"/> when building.
        /// For example, the <see cref="CreateFromFactoryMethodAttribute"/> may decorate a TestFixture class which provides
        /// itself as the <c>searchAnchor</c>.
        /// </summary>
        /// <param name="targetType">The type, the creating of an instance of which should be done by factory method</param>
        /// <param name="factoryMethodName">The factory method to call each time we try to create a <paramref name="targetType"/></param>
        /// <param name="args">any arguments needed for the factory method</param>
        public CreateFromFactoryMethodAttribute(Type targetType, string factoryMethodName, params object[] args)
        {
            this.targetType = targetType;
            this.factoryMethodName = factoryMethodName;
            this.args = args;
            if (factoryClass != null) { EnsureFactoryMethodElseThrow(factoryClass, null); }
        }

        /// <summary>
        /// Called from <see cref="CreateInstance.Of(Type, IEnumerable{IActivateAnythingRule}, IEnumerable{Type}, object)"/>
        /// to construct an instance of Type <paramref name="type"/>
        /// </summary>
        /// <param name="type">the Type to be instantiated</param>
        /// <param name="typesWaitingToBeBuilt">The 'stack' of types we are trying to build grows as instantiating a type 
        /// recursively requires the instantion of its constructor dependencies.
        /// This parameter is for the benefit of recursive rules whose strategy may vary depending on what we're trying to build.
        /// </param>
        /// <param name="searchAnchor">If no <see cref="factoryClass"/> was specified in the constructor, then the <c>searchAnchor</c>
        /// must declare a method named <see cref="factoryMethodName"/>.</param>
        /// <returns>An instance of type <paramref name="type"/> if possible, null if unable to construct one.</returns>
        public object CreateInstance(Type type, IEnumerable<Type> typesWaitingToBeBuilt, object searchAnchor)
        {
            if(type != targetType) {return null;}
            //
            if(factoryClass == null && searchAnchor == null)
            {
                throw new InvalidOperationException("You called CreateFromFactoryMethod.CreateInstance() without " +
                    "specifying a Type for the Factory. Either specify a Type in the CreateFromFactoryMethod constructor, " +
                    "or use an AnythingActivator with a searchAnchor which declares a factoryMethod.");
            }
            //
            object factory = searchAnchor 
                             ?? new AnythingActivator(
                                        ActivateAnythingDefaultRulesAttribute.DefaultFindConcreteTypeRuleSequence
                                            .Union(
                                                (IEnumerable<IActivateAnythingRule>)
                                                    new[]
                                                    {
                                                        new ChooseConstructorWithFewestParametersAttribute()
                                                    }
                                                )).Of(factoryClass); 
                             //nb if the factory method is static, then it's okay for factory to be null.

            Type factoryClassToUse = factory==null ? factoryClass : factory.GetType();

            var m = EnsureFactoryMethodElseThrow(factoryClassToUse, searchAnchor);
            //
            return m.Invoke(factory,args);
        }

        MethodInfo EnsureFactoryMethodElseThrow(Type factoryClassToUse, object searchAnchor)
        {
            var m = factoryClassToUse.GetMethod(factoryMethodName);
            //
            if(m == null)
            {
                throw new InvalidOperationException(
                    string.Format(MethodNotFoundFormat,
                                  targetType,
                                  factoryClass,
                                  factoryMethodName,
                                  searchAnchor,
                                  factoryClassToUse));
            }
            if(!m.ReturnType.IsAssignableFrom(targetType))
            {
                throw new ArgumentOutOfRangeException(targetType.FullName,
                                                      string.Format(ReturnTypeNotAssignableToTargetFormat,
                                                                    targetType,
                                                                    factoryClassToUse,
                                                                    factoryMethodName,
                                                                    m.ReturnType));
            }
            return m;
        }

        const string ReturnTypeNotAssignableToTargetFormat="BuildFromMethod({0},{1},{2}) doesn't work because {0} is not assignable to the return type {3} of {1}.{2}";
        const string MethodNotFoundFormat = "You asked for BuildFromFactory({0},{1},{2},...).CreateInstance(searchAnchor:{3}) but there is no method {4}.{2}";
    }
}
