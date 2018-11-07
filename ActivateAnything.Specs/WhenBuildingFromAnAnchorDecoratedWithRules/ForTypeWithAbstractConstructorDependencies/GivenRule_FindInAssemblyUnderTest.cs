using Xunit;
using TestBase;
using TestCases;

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