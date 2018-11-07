using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using TestBase;
using Assert = TestBase.Assert;

namespace ActivateAnything.Specs.WhenBuildingAnInstance
{
    public class GivenCustomCreateRuleForAType
    {
        [Fact]
        public void ThenI_UseIt_WhenBuildingAClass()
        {
            var customObject = new AClass();
            var result =
                CreateInstance.Of<AClass>(ActivateAnythingDefaultRulesAttribute.AllDefaultRules.Union(new[] {new CustomCreateInstanceRuleFor<AClass>(customObject)}));
            //
            Assert.That(customObject==result);
        }

        [Fact]
        public void ThenI_UseIt_WhenBuildingAString()
        {
            var result =
                CreateInstance.Of<string>(ActivateAnythingDefaultRulesAttribute.AllDefaultRules.Union(new[] {new CustomCreateInstanceRuleFor<string>("ACustomString")}));
            //
            result.ShouldBe("ACustomString");
        }

        class CustomCreateInstanceRuleFor<T> : IActivateAnythingCreateInstanceRule
        {
            public CustomCreateInstanceRuleFor(T value) { this.value = value; }
            public object CreateInstance(Type type, IEnumerable<Type> typesWaitingToBeBuilt, object anchorAssemblyType = null) { return value; }
            readonly T value;
        }

        class AClass { }
    }
}