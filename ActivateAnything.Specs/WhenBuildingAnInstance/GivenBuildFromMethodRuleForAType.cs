using System;
using System.Linq;
using TestBase;
using Xunit;
using Assert = TestBase.Assert;

namespace ActivateAnything.Specs.WhenBuildingAnInstance
{
    public class GivenBuildFromMethodRuleForAType
    {
        [Fact]
        public void ThenI_UseIt_WhenBuilding()
        {
            const string aCustomString = "ACustomString";
            var result =
                CreateInstance.Of<AClass>(
                    ActivateAnythingDefaultRulesAttribute.AllDefaultRules
                        .Union(
                                new[] { new CreateFromFactoryAttribute(typeof(AClass),typeof(AFactory),"BuildMethod", aCustomString) }
                        ));
            //
            Assert.That(aCustomString, x=>x==result.Aparameter);
        }

        [Fact]
        public void Then_BuildFromFactoryAttribute_ThrowsFromConstructor_GivenFactoryClassAndMissingMethod()
        {
            Assert.Throws<InvalidOperationException>(() =>
                                                     {
                                                         new CreateFromFactoryAttribute(typeof(AClass), typeof(AFactory), "BuildMethodNameWhichDoesntExist");
                                                     });
        }

        [Fact]
        public void ThenI_Throw_GivenRequestorObjectAndMissingMethod()
        {
            var rules = new[] {new CreateFromFactoryAttribute(typeof(AClass), "BuildMethodNameWhichDoesntExist")};
            //
            Assert.Throws<InvalidOperationException>(
                () => CreateInstance.Of<AClass>(ActivateAnythingDefaultRulesAttribute.AllDefaultRules.Union(rules))
                );
            Assert.Throws<InvalidOperationException>(
                () => CreateInstance.Of<AClass>(ActivateAnythingDefaultRulesAttribute.AllDefaultRules.Union(rules),this)
                );
        }


        class AClass {
            public readonly string Aparameter;
            public AClass(string aparameter) { Aparameter = aparameter; }
        }

        class AFactory
        {
            public AClass BuildMethod(string aparameter) { return new AClass(aparameter); }
        }
    }
}