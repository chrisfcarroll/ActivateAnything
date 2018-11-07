using Xunit;
using TestBase;
using TestCases;
using TestCases.AReferencedAssembly;

namespace ActivateAnything.Specs.WhenTestBaseBuildsUsingRuleAttributes.ForTypeWithAbstractConstructorDependencies
{
	[FindInAssembliesReferencedByAnchorAssembly]
    public class GivenRule_FindInAssembliesReferencedByAssemblyUnderTest : TestBaseFor<ClassWith1ConstructorParam<INterfaceWithClassInReferencedAssembly>>
    {
        [Fact]
        public void ThenI_FindConcreteTypeForInterfaceInReferencedAssemblies()
        {
            UnitUnderTest.ShouldNotBeNull();
            UnitUnderTest.ShouldBeAssignableTo<ClassWith1ConstructorParam<INterfaceWithClassInReferencedAssembly>>();
        }
    }
}
