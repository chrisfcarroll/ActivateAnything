using TestBase;
using TestCases;
using TestCases.AReferencedAssembly;
using Xunit;

namespace ActivateAnything.Specs.WhenBuildingFromAnAnchorDecoratedWithRules.ForTypeWithAbstractConstructorDependencies
{
    [FindInAssembly("TestCases.ANotReferencedAssembly")]
    public class GivenRule_FindInAssembly_And_NameOfAssemblyInBaseDirectory : TestBaseFor<
        ClassWith1ConstructorParam<INterfaceWithClassInNotReferencedAssembly>>
    {
        [Fact]
        public void AAFindsConcreteTypeForInterfaceInNamedAssembly()
        {
            UnitUnderTest.ShouldNotBeNull();

            UnitUnderTest.ShouldBeAssignableTo<ClassWith1ConstructorParam<INterfaceWithClassInNotReferencedAssembly>>();

            UnitUnderTest
           .Param1.GetType()
           .Assembly.FullName
           .ShouldStartWith("TestCases.ANotReferencedAssembly");
        }
    }
}
