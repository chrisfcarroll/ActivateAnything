using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ActivateAnything
{
    /// <summary>
    ///     <para>
    ///         <strong>Warning:</strong> "Referenced Assembly" -- as in <see cref="Assembly.GetReferencedAssemblies" /> --
    ///         may not mean what you think. Adding an assembly as a reference to a project does not make
    ///         it a referenced assembly. Only if your code actually refers to a Type from that assembly
    ///         does it become referenced.
    ///     </para>
    ///     <strong>Note</strong> that this strategy will consider the referenced assemblies of not one but several assemblies:
    ///     <list type="bullet">
    ///         <item>
    ///             The assemblies referenced by the 'Assembly Under Test', that is the assembly containing the Type under
    ///             test.
    ///         </item>
    ///         <item>
    ///             Then there are the assemblies referenced by the assemblies containing the Types which we (recursively)
    ///             need in under to construct that Type under test.
    ///         </item>
    ///     </list>
    /// </summary>
    public class FindInAssembliesReferencedByAnchorAssembly : FindTypeRule
    {
        /// <summary>
        ///     Assemblies will not be searched if the name begins with any of these.
        ///     Defaults to <see cref="DefaultAssembliesToIgnore.ByName" />
        /// </summary>
        public string[] IgnoreAssembliesWhereNameStartsWith { get; set; } = DefaultAssembliesToIgnore.ByName;

        /// <inheritdoc />
        public override Type FindTypeAssignableTo(
        Type type,
        IEnumerable<Type> typesWaitingToBeBuilt = null,
        object anchor = null)
        {
            return anchor == null
            ? null
            : FindType(
            t => !t.IsAbstract && !t.IsInterface && type.IsAssignableFrom(t),
            typesWaitingToBeBuilt,
            anchor.GetType()
            );
        }

        /// <inheritdoc />
        public override Type FindTypeAssignableTo(
        string typeName,
        IEnumerable<Type> typesWaitingToBeBuilt = null,
        object anchor = null)
        {
            return anchor == null
            ? null
            : FindType(
            t => !t.IsAbstract && !t.IsInterface && t.FullName.EndsWith(typeName),
            typesWaitingToBeBuilt,
            anchor.GetType());
        }

        Type FindType(Func<Type, bool> filterBy, IEnumerable<Type> typesWaitingToBeBuilt, Type anchorType)
        {
            var typesFromWhichToSearch =
            new[] {anchorType}
            .Union(typesWaitingToBeBuilt ?? new Type[0])
            .Where(x => x != null);

            var assembliesToIgnore = IgnoreAssembliesWhereNameStartsWith ?? DefaultAssembliesToIgnore.ByName;

            var assemblyNamesToSearch =
            typesFromWhichToSearch
            .SelectMany(t => t.Assembly.GetReferencedAssemblies())
            .Where(a => !assembliesToIgnore.Any(n => a.FullName.StartsWith(n)));

            var allTypesInReferencedAssemblies =
            assemblyNamesToSearch
            .Select(name => Assembly.Load(name))
            .SelectMany(a => a.GetTypes());

            return allTypesInReferencedAssemblies.Where(filterBy).FirstOrDefault();
        }
    }
}
