using Xunit;
using TestBase;
using TestCases;

namespace ActivateAnything.Specs.WhenTestBaseBuildsUsingRuleAttributes
{
    public class Given_TypeWithNoConstructorDependencies : TestBaseFor<ClassWithDefaultConstructor>
    {
        [Fact]
        public void ThenI_CreateUnitUnderTestAsAnInstanceOfT()
        {
            UnitUnderTest.ShouldNotBeNull();
            UnitUnderTest.ShouldBeAssignableTo<ClassWithDefaultConstructor>();
        }

    }
}