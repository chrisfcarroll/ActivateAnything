using Xunit;
using TestBase;
using TestCases;

namespace ActivateAnything.Specs.WhenTestBaseBuildsUsingRuleAttributes.ForTypeWithAbstractConstructorDependencies
{
	[FindInAnyAssemblyReferencedByAssemblyContainingType]
    public class GivenRule_FindInAssemblyUnderTest : TestBaseFor<ClassWith1ConstructorParam<INterfaceWithClassInSameAssembly>>
    {
        [Fact]
        public void ThenI_FindConcreteTypeForInterfaceInSameAssembly()
        {
            UnitUnderTest.ShouldNotBeNull();
            UnitUnderTest.ShouldBeAssignableTo<ClassWith1ConstructorParam<INterfaceWithClassInSameAssembly>>();
        }
    }
}