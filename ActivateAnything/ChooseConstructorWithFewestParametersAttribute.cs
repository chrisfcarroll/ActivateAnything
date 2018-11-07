using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ActivateAnything
{
    public class ChooseConstructorWithFewestParametersAttribute : ChooseConstructorRuleAttribute
    {

        public bool PreferPublic { get; set; }

        public override ConstructorInfo ChooseConstructor(Type type, IEnumerable<Type> typesWaitingToBeBuilt, object anchorAssemblyType=null)
        {
            return type.GetConstructors()
                .OrderByDescending(c => PreferPublic && c.IsPublic)
                .ThenBy(c => c.GetParameters().Length)
                .FirstOrDefault();
        }
    }
}