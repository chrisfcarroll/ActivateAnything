using TestBase;
using TestCases;
using Xunit;

namespace ActivateAnything.Specs.WhenBuildingFromAnAnchorDecoratedWithRules.ForTypeWithAbstractConstructorDependencies
{
    [FindInAnchorAssembly]
    public class GivenRule_FindInTestFixturesAssembly : TestBaseFor<ClassWith1ConstructorParam<INterface>>
    {
        [Fact]
        public void AAFindsConcreteTypeForInterfaceInSameAssembly()
        {
            UnitUnderTest.ShouldNotBeNull();
            UnitUnderTest.ShouldBeAssignableTo<ClassWith1ConstructorParam<INterface>>();
            typeof(NterfaceImplementedInTestAssembly).ShouldBe(UnitUnderTest.Param1.GetType());
        }
    }

    class NterfaceImplementedInTestAssembly : INterface{}
}