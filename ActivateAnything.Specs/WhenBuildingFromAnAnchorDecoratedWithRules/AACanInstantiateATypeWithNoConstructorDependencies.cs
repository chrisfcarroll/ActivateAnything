using Xunit;
using TestBase;
using TestCases;

namespace ActivateAnything.Specs.WhenBuildingFromAnAnchorDecoratedWithRules
{
    public class AACanInstantiateATypeWithNoConstructorDependencies : TestBaseFor<ClassWithDefaultConstructor>
    {
        [Fact]
        public void AndDoesSo()
        {
            UnitUnderTest.ShouldNotBeNull();
            UnitUnderTest.ShouldBeAssignableTo<ClassWithDefaultConstructor>();
        }

    }
}