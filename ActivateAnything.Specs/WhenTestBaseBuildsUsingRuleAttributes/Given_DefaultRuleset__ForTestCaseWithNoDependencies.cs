using Xunit;
using TestBase;
using TestCases;

namespace ActivateAnything.Specs.WhenTestBaseBuildsUsingRuleAttributes
{
	[ActivateAnythingDefaultRules]
    public class Given_DefaultRuleset__ForTestCaseWithNoDependencies : TestBaseFor<ClassWithDefaultConstructor>
    {
        [Fact]
        public void AndI_BuildRequestedType()
        {
            UnitUnderTest
                .ShouldNotBeNull()
                .ShouldBeAssignableTo<ClassWithDefaultConstructor>();
        }
    }
}
