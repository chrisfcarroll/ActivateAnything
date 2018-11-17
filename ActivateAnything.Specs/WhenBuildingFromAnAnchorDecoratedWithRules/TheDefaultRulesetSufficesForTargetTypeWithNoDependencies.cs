using TestBase;
using TestCases;
using Xunit;

namespace ActivateAnything.Specs.WhenBuildingFromAnAnchorDecoratedWithRules
{
    [ActivateAnythingDefaultRules]
    public class TheDefaultRulesetSufficesForTargetTypeWithNoDependencies : TestBaseFor<ClassWithDefaultConstructor>
    {
        [Fact]
        public void AndAABuildsRequestedType()
        {
            UnitUnderTest
                .ShouldNotBeNull()
                .ShouldBeAssignableTo<ClassWithDefaultConstructor>();
        }
    }
}