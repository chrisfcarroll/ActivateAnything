using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ActivateAnything
{
    /// <summary>
    /// Searches files with a .dll or .exe extension in the <see cref="AppDomain.BaseDirectory"/> directory and
    /// finds Types in them.
    /// In a typical edit/test from the IDE usage, the BaseDirectory will the the Test Projects bin\Debug directory.
    /// </summary>
    public class FindInAssemblyAttribute : ActivateAnythingFindConcreteTypeRuleAttribute
    {
        public FindInAssemblyAttribute(string assemblyName){ this.assemblyName = assemblyName; }

        static readonly DirectoryInfo BaseDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

        readonly string assemblyName;

        public override Type FindTypeAssignableTo(Type type, IEnumerable<Type> typesWaitingToBeBuilt = null, object testFixtureType = null)
        {
            return FindTypeAssignableTo(t => !t.IsAbstract && !t.IsInterface && type.IsAssignableFrom(t));
        }
        public override Type FindTypeAssignableTo(string typeName, IEnumerable<Type> typesWaitingToBeBuilt = null, object searchAnchor = null)
        {
            return FindTypeAssignableTo(t => !t.IsAbstract && !t.IsInterface && t.FullName.EndsWith(typeName));
        }

        Type FindTypeAssignableTo(Func<Type, bool> filterBy)
        {
            if(assemblyName.Contains('*') || assemblyName.Contains('?')) { return FindBestMatchFromAssembliesInBaseDirectory(filterBy); } else
            {
                try { return Assembly.Load(assemblyName).GetTypes().First(filterBy); } catch(Exception)
                {
                    try { return Assembly.LoadFrom(assemblyName).GetTypes().First(filterBy); } catch(Exception) {
                        return FindBestMatchFromAssembliesInBaseDirectory(filterBy);
                    }
                }
            }
        }

        Type FindBestMatchFromAssembliesInBaseDirectory(Func<Type, bool> filterBy)
        {
            var possibleAssembliesInApplicationBase = BaseDirectory.EnumerateFiles(assemblyName + ".dll").Union(BaseDirectory.EnumerateFiles(assemblyName + ".exe")).OrderByDescending(a => a.FullName.Length);
            //Assembly name can contain wildcards 
            var allTypesInBaseDirectory = possibleAssembliesInApplicationBase.Select(a => 
            {
                try
                {
                    return Assembly.Load(Path.GetFileNameWithoutExtension(a.Name));
                }
                catch
                {
                    return null;
                }
            }).Where(a => a != null).SelectMany(a => a.GetTypes());
            var relevantTypes = allTypesInBaseDirectory.Where(filterBy);
            return relevantTypes.FirstOrDefault();
        }
    }
}