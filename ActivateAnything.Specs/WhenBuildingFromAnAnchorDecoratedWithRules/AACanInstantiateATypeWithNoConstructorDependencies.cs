using TestBase;
using TestCases;
using Xunit;

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