using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ActivateAnything
{
    /// <summary>
    ///     Attempts to <see cref="Assembly.Load(string)" /> an <see cref="Assembly" /> with the given name and search it for
    ///     the
    ///     requested Type. Given a wildcard name, searches the files with a .dll or .exe extension in the
    ///     <see cref="AppDomain.BaseDirectory" /> directory.
    ///     When used from a Test project, the BaseDirectory will typically be the {TestProject}\bin\Debug directory.
    /// </summary>
    public class FindInAssemblyAttribute : ActivateAnythingFindConcreteTypeRuleAttribute
    {
        static readonly DirectoryInfo BaseDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        readonly string assemblyName;
        readonly Func<Type, bool> filter;

        public FindInAssemblyAttribute(string assemblyName)
        {
            this.assemblyName = assemblyName;
            filter = t => !t.IsAbstract && !t.IsInterface;
        }

        protected FindInAssemblyAttribute(string assemblyName, Func<Type, bool> filter)
        {
            this.assemblyName = assemblyName;
            this.filter = filter;
        }

        public override Type FindTypeAssignableTo(
            Type type,
            IEnumerable<Type> typesWaitingToBeBuilt = null,
            object testFixtureType = null)
        {
            return FindTypeAssignableTo(t => filter(t) && type.IsAssignableFrom(t));
        }

        public override Type FindTypeAssignableTo(
            string typeName,
            IEnumerable<Type> typesWaitingToBeBuilt = null,
            object searchAnchor = null)
        {
            return FindTypeAssignableTo(t => filter(t) && t.FullName.EndsWith(typeName));
        }

        Type FindTypeAssignableTo(Func<Type, bool> filterBy)
        {
            if (assemblyName.Contains('*') || assemblyName.Contains('?'))
                return FindBestMatchFromAssembliesInBaseDirectory(filterBy);
            else
                try
                {
                    return Assembly.Load(assemblyName).GetTypes().First(filterBy);
                } catch (Exception)
                {
                    try
                    {
                        return Assembly.LoadFrom(assemblyName).GetTypes().First(filterBy);
                    } catch (Exception)
                    {
                        return FindBestMatchFromAssembliesInBaseDirectory(filterBy);
                    }
                }
        }

        Type FindBestMatchFromAssembliesInBaseDirectory(Func<Type, bool> filterBy)
        {
            var possibleAssembliesInApplicationBase = BaseDirectory.EnumerateFiles(assemblyName + ".dll")
                .Union(BaseDirectory.EnumerateFiles(assemblyName + ".exe"))
                .OrderByDescending(a => a.FullName.Length);
            //Assembly name can contain wildcards 
            var allTypesInBaseDirectory = possibleAssembliesInApplicationBase.Select(a =>
                {
                    try
                    {
                        return Assembly.Load(Path.GetFileNameWithoutExtension(a.Name));
                    } catch
                    {
                        return null;
                    }
                })
                .Where(a => a != null)
                .SelectMany(a => a.GetTypes());
            var relevantTypes = allTypesInBaseDirectory.Where(filterBy);
            return relevantTypes.FirstOrDefault();
        }
    }
}