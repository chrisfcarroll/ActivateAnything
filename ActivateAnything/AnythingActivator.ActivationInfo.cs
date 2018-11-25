using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

#if NET45
#elif NET40
#endif


namespace ActivateAnything
{
    #pragma warning disable 1591
    /// <summary>A struct recording information about an activation done by <see cref="AnythingActivator" /></summary>
    public struct ActivationInfo
    {
        public string How;
        public IEnumerable<Type> TypeStack;
        public ConstructorInfo ConstructorInfo;
        public string[] Parameters;

        public static ActivationInfo InstanceRule(IEnumerable<Type> typeStack)
        {
            return new ActivationInfo
                   {
                   How             = "ActivateInstanceRule",
                   TypeStack       = typeStack,
                   ConstructorInfo = null,
                   Parameters      = null
                   };
        }

        public static ActivationInfo ValueType(IEnumerable<Type> typeStack)
        {
            return new ActivationInfo
                   {
                   How             = "ValueType",
                   TypeStack       = typeStack,
                   ConstructorInfo = null,
                   Parameters      = null
                   };
        }

        public static ActivationInfo Constructed(
            IEnumerable<Type> typeStack,
            ConstructorInfo   constructorInfo,
            params object[]   parameters)
        {
            return new ActivationInfo
                   {
                   How             = "Constructed",
                   TypeStack       = typeStack,
                   ConstructorInfo = constructorInfo,
                   Parameters      = parameters?.Select(p => p?.ToString()).ToArray() ?? new string[0]
                   };
        }

        public string ToString(ActivationInfoFormat format)
        {
            switch (format)
            {
                case 0:                                 return ToString(abbreviateTypePrefixesBeginningWith: null);
                case ActivationInfoFormat.TypeFullName: return ToString();
                case ActivationInfoFormat.TypeAssemblyQualifiedName:
                default:
                    return ToString(t => t.AssemblyQualifiedName);
            }
        }

        public override string ToString()
        {
            var parameters = Parameters == null ? "()" : "( " + string.Join(",", Parameters) + " )";
            var typeStack  = TypeStack  == null ? "[]" : "[" + string.Join("->", TypeStack)  + " ]";

            return $"({How} {typeStack} {ConstructorInfo}{parameters})";
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public string ToString(string abbreviateTypePrefixesBeginningWith = @".*\.")
        {
            var regex = new Regex(abbreviateTypePrefixesBeginningWith ?? @".*\.");
            return ToString(t => regex.Replace(t.FullName ?? t.Name, ""));
        }

        string ToString(Func<Type, string> formatTypeNames)
        {
            var parameters = Parameters == null ? "()" : "( " + string.Join(",", Parameters) + " )";
            var typeStack = TypeStack == null
                            ? "[]"
                            : "[" + string.Join("->", TypeStack.Select(formatTypeNames)) + " ]";

            return $"({How} {typeStack} {ConstructorInfo}{parameters})";
        }

        public static ActivationInfo NoConstructor(IEnumerable<Type> typesWaitingToBeBuilt)
        {
            return new ActivationInfo
                   {
                   How             = "System.Activator",
                   TypeStack       = typesWaitingToBeBuilt,
                   ConstructorInfo = null,
                   Parameters      = null
                   };
        }
    }

    public enum ActivationInfoFormat
    {
        TypeName = 0,
        TypeFullName = 1,
        TypeAssemblyQualifiedName = 2
    }
    #pragma warning restore 1591
}
