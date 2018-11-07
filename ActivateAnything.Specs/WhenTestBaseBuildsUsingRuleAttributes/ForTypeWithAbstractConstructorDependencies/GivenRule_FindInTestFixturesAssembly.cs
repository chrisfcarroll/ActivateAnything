using Xunit;
using TestBase;
using TestCases;

namespace ActivateAnything.Specs.WhenTestBaseBuildsUsingRuleAttributes.ForTypeWithAbstractConstructorDependencies
{
	[FindOnlyInAnchorAssembly]
    public class GivenRule_FindInTestFixturesAssembly : TestBaseFor<ClassWith1ConstructorParam<INterface>>
    {
        [Fact]
        public void ThenI_FindConcreteTypeForInterfaceInSameAssembly()
        {
            UnitUnderTest.ShouldNotBeNull();
            UnitUnderTest.ShouldBeAssignableTo<ClassWith1ConstructorParam<INterface>>();
            typeof(NterfaceImplementedInTestAssembly).ShouldBe( UnitUnderTest.Param1.GetType() );
        }
    }

    class NterfaceImplementedInTestAssembly : INterface {}
}