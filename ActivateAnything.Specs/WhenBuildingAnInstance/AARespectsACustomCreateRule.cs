using System;
using System.Collections.Generic;
using System.Linq;
using TestBase;
using Xunit;
using Assert = TestBase.Assert;

namespace ActivateAnything.Specs.WhenBuildingAnInstance
{
    public class AARespectsACustomCreateRule
    {
        class CustomCreateInstanceRuleFor<T> : IActivateAnythingCreateInstanceRule
        {
            readonly T value;
            public CustomCreateInstanceRuleFor(T value) { this.value = value; }

            public object CreateInstance(Type type, IEnumerable<Type> typesWaitingToBeBuilt, object searchAnchor = null)
            {
                return value;
            }
        }

        class AClass
        {
        }

        [Fact]
        public void ForAClass()
        {
            var customObject = new AClass();
            var result =
                new AnythingActivator(
                        ActivateAnythingDefaultRulesAttribute.AllDefaultRules
                            .Union(
                                new[] {new CustomCreateInstanceRuleFor<AClass>(customObject)}))
                    .Of<AClass>();
            //
            Assert.That(customObject == result);
        }

        [Fact]
        public void ForAString()
        {
            var result =
                new AnythingActivator(
                        ActivateAnythingDefaultRulesAttribute.AllDefaultRules
                            .Union(
                                new[] {new CustomCreateInstanceRuleFor<string>("ACustomString")}))
                    .Of<string>();
            //
            result.ShouldBe("ACustomString");
        }
    }
}