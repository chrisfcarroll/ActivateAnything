using Xunit;
using TestBase;
using TestCases;
using TestCases.AReferencedAssembly;
using Assert = TestBase.Assert;

namespace ActivateAnything.Specs.WhenBuildingFromAnAnchorDecoratedWithRules.ForTypeWithAbstractConstructorDependencies
{
	[FindInDirectory]
    public class GivenRule_FindInAssembliesInBaseDirectory : TestBaseFor<ClassWith1ConstructorParam<INterfaceWithClassInNotReferencedAssembly>>
    {
        [Fact]
        public void AAFindsConcreteTypeForInterfaceInAssembliesInBaseDirectory()
        {
            UnitUnderTest.ShouldNotBeNull();
            UnitUnderTest.ShouldBeAssignableTo<ClassWith1ConstructorParam<INterfaceWithClassInNotReferencedAssembly>>();
        }

        [Fact]
        public void AAFindsConcreteTypeEvenIfTheAssemblyIsntReferenced()
        {
            Assert.That(UnitUnderTest.Param1.GetType().Assembly.FullName.Contains("TestCases.ANotReferencedAssembly"));
        }
    }
}