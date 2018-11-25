using TestBase;
using TestCases;
using Xunit;

namespace ActivateAnything.Specs.WhenBuildingFromAnAnchorDecoratedWithRules.ForTypeWithAbstractConstructorDependencies
{
    [FindInAssembly("ActivateAnything.Specs")]
    public class GivenRule_FindInAssembly_And_NameOfAnAlreadyLoadedAssembly : TestBaseFor<
    ClassWith1ConstructorParam<INterfaceWithFakeInTestAssembly>>
    {
        [Fact]
        public void AAFindsConcreteTypeForInterfaceInNamedAssembly()
        {
            UnitUnderTest.ShouldNotBeNull();

            UnitUnderTest.ShouldBeAssignableTo<ClassWith1ConstructorParam<INterfaceWithFakeInTestAssembly>>();
            typeof(NterfaceWithFakeInTestAssembly).ShouldBe(UnitUnderTest.Param1.GetType());
        }
    }
}
