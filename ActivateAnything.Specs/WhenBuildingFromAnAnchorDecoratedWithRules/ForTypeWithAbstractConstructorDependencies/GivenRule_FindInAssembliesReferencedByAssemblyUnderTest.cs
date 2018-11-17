using TestBase;
using TestCases;
using TestCases.AReferencedAssembly;
using Xunit;

namespace ActivateAnything.Specs.WhenBuildingFromAnAnchorDecoratedWithRules.ForTypeWithAbstractConstructorDependencies
{
    [FindInAssembliesReferencedByAnchorAssembly]
    public class GivenRule_FindInAssembliesReferencedByAssemblyUnderTest : TestBaseFor<
        ClassWith1ConstructorParam<INterfaceWithClassInReferencedAssembly>>
    {
        [Fact]
        public void AAFindsConcreteTypeForInterfaceInReferencedAssemblies()
        {
            UnitUnderTest.ShouldNotBeNull();
            UnitUnderTest.ShouldBeAssignableTo<ClassWith1ConstructorParam<INterfaceWithClassInReferencedAssembly>>();
        }
    }
}