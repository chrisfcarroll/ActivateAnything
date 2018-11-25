using System.Linq;
using TestBase;
using Xunit;
using Assert = TestBase.Assert;

namespace ActivateAnything.Specs.WhenBuildingAnInstance
{
    public class AARespectsActivateForType
    {

        class AClass{}

        [Fact]
        public void ForAClass()
        {
            var myObject = new AClass();
            var result =
                new AnythingActivator(
                        ActivateDefaultRules.AllDefaultRules
                            .Union(
                                new[] {new ActivateForType<AClass>(myObject)}))
                    .New<AClass>();
            //
            Assert.That(myObject == result);
        }

        [Fact]
        public void ForAString()
        {
            var result =
                new AnythingActivator(
                        ActivateDefaultRules.AllDefaultRules
                            .Union(
                                new[] {new ActivateForType<string>("ACustomString")}))
                    .New<string>();
            //
            result.ShouldBe("ACustomString");
        }
    }
}