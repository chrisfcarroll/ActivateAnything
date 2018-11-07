using Xunit;
using TestBase;
using TestCases;

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
