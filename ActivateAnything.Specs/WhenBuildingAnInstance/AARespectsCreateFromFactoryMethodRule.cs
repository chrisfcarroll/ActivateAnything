using System;
using System.Linq;
using TestBase;
using Xunit;
using Assert = TestBase.Assert;

namespace ActivateAnything.Specs.WhenBuildingAnInstance
{
    public class AARespectsCreateFromFactoryMethodRule
    {
        class AClass
        {
            public readonly string Aparameter;
            public AClass(string aparameter) { Aparameter = aparameter; }
        }

        class AFactory
        {
            public AClass BuildMethod(string aparameter) { return new AClass(aparameter); }
        }

        [Fact]
        public void ButIfFactoryMethodIsMissing_ThenAAThrows_WithOrWithoutAnAnchor()
        {
            var rules = new[] {new CreateFromFactoryMethodAttribute(typeof(AClass), "BuildMethodNameWhichDoesntExist")};
            //
            Assert.Throws<InvalidOperationException>(
                () => new AnythingActivator(ActivateAnythingDefaultRulesAttribute.AllDefaultRules.Union(rules)).Of<AClass>()
            );
            Assert.Throws<InvalidOperationException>(
                () => new AnythingActivator(this, ActivateAnythingDefaultRulesAttribute.AllDefaultRules.Union(rules))
                    .Of<AClass>()
            );
        }

        [Fact]
        public void ButIfFactoryMethodIsMissing_ThenCreateFromFactoryMethodAttributeConstructorThrows()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                new CreateFromFactoryMethodAttribute(typeof(AClass),
                    typeof(AFactory),
                    "BuildMethodNameWhichDoesntExist");
            });
        }

        [Fact]
        public void ForAClass()
        {
            const string aCustomString = "ACustomString";
            var result =
                new AnythingActivator(
                    ActivateAnythingDefaultRulesAttribute.AllDefaultRules
                        .Union(
                            new[]
                            {
                                new CreateFromFactoryMethodAttribute(
                                    typeof(AClass),
                                    typeof(AFactory),
                                    "BuildMethod",
                                    aCustomString)
                            }
                        )).Of<AClass>();
            //
            result.Aparameter.ShouldBe(aCustomString);
        }
    }
}