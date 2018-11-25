using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ActivateAnything
{
    /// <summary>
    ///     Finds types in files with a .dll or .exe extension in the
    ///     <see cref="AppDomain.CurrentDomain" />.<see cref="AppDomain.BaseDirectory" />.
    ///     When used by a test project, the BaseDirectory will usually be the msbuild output
    ///     directory, e.g. the {TestProject}\bin\Debug\ directory.
    /// </summary>
    public class FindInDirectory : FindTypeRule
    {
        static readonly DirectoryInfo BaseDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

        /// <summary>
        ///     Default value: <c>{ "System", "nunit", "Moq" }</c>
        /// </summary>
        public string[] IgnoreAssembliesWhereNameStartsWith { get; set; } = DefaultAssembliesToIgnore.ByName;

        /// <inheritdoc />
        public override Type FindTypeAssignableTo(
            Type              type,
            IEnumerable<Type> typesWaitingToBeBuilt = null,
            object            searchAnchor          = null)
        {
            return FindTypeAssignableTo(t => !t.IsAbstract && !t.IsInterface && type.IsAssignableFrom(t));
        }

        /// <inheritdoc />
        public override Type FindTypeAssignableTo(
            string            typeName,
            IEnumerable<Type> typesWaitingToBeBuilt = null,
            object            searchAnchor          = null)
        {
            return FindTypeAssignableTo(t => !t.IsAbstract && !t.IsInterface && t.FullName.EndsWith(typeName));
        }


        Type FindTypeAssignableTo(Func<Type, bool> filterBy)
        {
            var possibleAssembliesInApplicationBase =
            BaseDirectory.EnumerateFiles("*.dll")
                         .Union(BaseDirectory.EnumerateFiles("*.exe"));

            var assembliesToIgnore = IgnoreAssembliesWhereNameStartsWith ?? DefaultAssembliesToIgnore.ByName;

            var allTypesInBaseDirectory =
            possibleAssembliesInApplicationBase
           .Where(a => !assembliesToIgnore.Any(ia => a.Name.StartsWith(ia)))
           .Select(a =>
                   {
                       try { return Assembly.Load(Path.GetFileNameWithoutExtension(a.Name)); } catch { return null; }
                   })
           .Where(a => a != null)
           .SelectMany(a => a.GetTypes());

            return allTypesInBaseDirectory.Where(filterBy).FirstOrDefault();
        }
    }
}
