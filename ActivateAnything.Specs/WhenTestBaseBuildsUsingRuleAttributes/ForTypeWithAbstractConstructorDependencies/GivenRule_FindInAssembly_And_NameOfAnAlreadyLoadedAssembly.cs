using Xunit;
using TestBase;
using TestCases;

namespace ActivateAnything.Specs.WhenTestBaseBuildsUsingRuleAttributes.ForTypeWithAbstractConstructorDependencies
{
    [FindInAssembly("ActivateAnything.Specs")]
    public class GivenRule_FindInAssembly_And_NameOfAnAlreadyLoadedAssembly : TestBaseFor<ClassWith1ConstructorParam<INterfaceWithFakeInTestAssembly>>
    {
        [Fact]
        public void ThenI_FindConcreteTypeForInterfaceInNamedAssembly()
        {
            UnitUnderTest.ShouldNotBeNull();

            UnitUnderTest.ShouldBeAssignableTo<ClassWith1ConstructorParam<INterfaceWithFakeInTestAssembly>>();
            typeof(NterfaceWithFakeInTestAssembly).ShouldBe( UnitUnderTest.Param1.GetType() );
        }
    }
}