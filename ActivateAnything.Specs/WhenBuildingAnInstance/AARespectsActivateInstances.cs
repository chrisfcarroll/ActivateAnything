using System;
using System.Linq;
using System.Linq.Expressions;
using TestBase;
using Xunit;
using Assert = TestBase.Assert;

namespace ActivateAnything.Specs.WhenBuildingAnInstance
{
    public class AARespectsActivateInstances
    {
        class AClass{}

        [Fact]
        public void ForAClass()
        {
            var myObject = new AClass();
            var result =
                new AnythingActivator(AnythingActivator.DefaultRules.After(new ActivateInstances(myObject)))
                    .New<AClass>();
            //
            Assert.That(myObject == result);
        }


        [Fact]
        public void ForAString()
        {
            var result =
                new AnythingActivator(
                        ActivateDefaultRulesAttribute.AllDefaultRules
                            .After(
                                new[] {new ActivateInstances("ACustomString"), }))
                    .New<string>();
            //
            result.ShouldBe("ACustomString");
        }
        
        [Fact(Skip = "WIP Next")]
        public void ForAFuncOf()
        {
            var myObject = new AClass();
            var result =
                new AnythingActivator(AnythingActivator.DefaultRules.After(new ActivateInstances(myObject)))
                   .New<Func<AClass>>();
            //
            Assert.That(myObject == result());
        }
    }
}