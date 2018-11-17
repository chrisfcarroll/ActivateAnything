using TestBase;
using TestCases;
using Xunit;

namespace ActivateAnything.Specs.WhenBuildingFromAnAnchorDecoratedWithRules.ForTypeWithAbstractConstructorDependencies
{
    [FindInAnyAssemblyReferencedByAssemblyContainingType]
    public class GivenRule_FindInAssemblyUnderTest : TestBaseFor<ClassWith1ConstructorParam<INterfaceWithClassInSameAssembly>>
    {
        [Fact]
        public void AAFindsConcreteTypeForInterfaceInSameAssembly()
        {
            UnitUnderTest.ShouldNotBeNull();
            UnitUnderTest.ShouldBeAssignableTo<ClassWith1ConstructorParam<INterfaceWithClassInSameAssembly>>();
        }
    }
}