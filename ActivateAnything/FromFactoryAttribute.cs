using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ActivateAnything
{
    public class CreateFromFactoryAttribute : Attribute, IActivateAnythingCreateInstanceRule
    {
        readonly Type targetTypeToBuild;
        readonly Type factoryClass;
        readonly string factoryMethodName;
        readonly object[] args;

        /// <summary>
        /// Specifies a specific factory method to call 
        /// </summary>
        /// <param name="targetTypeToBuild">The type, the creating of an instance of which should be done by a factory method.</param>
        /// <param name="factoryClass">The class where the factory method can be found. 
        /// If <paramref name="factoryMethodName"/> is not static then it must be possible to create an instance (for instance, 
        /// by calling a constructor with no parameters, or with parameters we can recursively create)</param>
        /// <param name="factoryMethodName">The factory method to call each time we try to create a <paramref name="targetTypeToBuild"/></param>
        /// <param name="args">any arguments needed for the factory method</param>
        public CreateFromFactoryAttribute(Type targetTypeToBuild, Type factoryClass, string factoryMethodName, params object[] args)
        {
            this.targetTypeToBuild = targetTypeToBuild;
            this.factoryClass = factoryClass;
            this.factoryMethodName = factoryMethodName;
            this.args = args;
            if(factoryClass != null){ EnsureFactoryMethodElseThrow(factoryClass,null); }
        }
        /// <summary>
        /// Specifies a factory method to call. The method should exist on the anchorAssemblyType object passed to <see cref="Construct.InstanceOf"/> when building.
        /// In typical usage, the anchorAssemblyType object will be the TestFixture decorated with this instance of <see cref="BuildFromFactoryAttribute"/>
        /// </summary>
        /// <param name="targetTypeToBuild">The type, the creating of an instance of which should be done by factory method</param>
        /// <param name="factoryMethodName">The factory method to call each time we try to create a <paramref name="targetTypeToBuild"/></param>
        /// <param name="args">any arguments needed for the factory method</param>
        public CreateFromFactoryAttribute(Type targetTypeToBuild, string factoryMethodName, params object[] args)
        {
            this.targetTypeToBuild = targetTypeToBuild;
            this.factoryMethodName = factoryMethodName;
            this.args = args;
            if (factoryClass != null) { EnsureFactoryMethodElseThrow(factoryClass, null); }
        }

        public object CreateInstance(Type type, IEnumerable<Type> typesWaitingToBeBuilt, object anchorAssemblyType)
        {
            if(type != targetTypeToBuild) {return null;}
            //
            if(factoryClass == null && anchorAssemblyType == null)
            {
                throw new InvalidOperationException("You called ActivateFromFactoryAttributeAttribute.CreateInstance() with giving a System.Type for the Factory. Either specify a Type in the BuildFromFactory constructor, or ensure that a RequestedBy object is given in the call to CreateInstance.");
            }
            //
            object factory = anchorAssemblyType ?? ActivateAnything.CreateInstance.Of(factoryClass, ActivateAnythingDefaultRulesAttribute.DefaultFindConcreteTypeRuleSequence.Union((IEnumerable<IActivateAnythingRule>)(new[] {new ChooseConstructorWithFewestParametersAttribute()}))); //nb if the factory method is static, then it's okay for factory to be null.

            Type factoryClassToUse =   factory==null ? factoryClass : factory.GetType();

            var m = EnsureFactoryMethodElseThrow(factoryClassToUse, anchorAssemblyType);
            //
            return m.Invoke(factory,args);
        }

        MethodInfo EnsureFactoryMethodElseThrow(Type factoryClassToUse, object anchorAssemblyType)
        {
            var m = factoryClassToUse.GetMethod(factoryMethodName);
            //
            if(m == null)
            {
                throw new InvalidOperationException(
                    string.Format(MethodNotFoundFormat,
                                  targetTypeToBuild,
                                  factoryClass,
                                  factoryMethodName,
                                  anchorAssemblyType,
                                  factoryClassToUse));
            }
            if(!m.ReturnType.IsAssignableFrom(targetTypeToBuild))
            {
                throw new ArgumentOutOfRangeException(targetTypeToBuild.FullName,
                                                      string.Format(ReturnTypeNotAssignableToTargetFormat,
                                                                    targetTypeToBuild,
                                                                    factoryClassToUse,
                                                                    factoryMethodName,
                                                                    m.ReturnType));
            }
            return m;
        }

        const string ReturnTypeNotAssignableToTargetFormat="BuildFromMethod({0},{1},{2}) doesn't work because {0} is not assignable to the return type {3} of {1}.{2}";
        const string MethodNotFoundFormat = "You asked for BuildFromFactory({0},{1},{2},...).CreateInstance(anchorAssemblyType:{3}) but there is no method {4}.{2}";
    }
}
