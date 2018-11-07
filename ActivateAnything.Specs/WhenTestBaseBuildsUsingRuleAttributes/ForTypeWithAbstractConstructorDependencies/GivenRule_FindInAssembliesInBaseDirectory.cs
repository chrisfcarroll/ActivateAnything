using Xunit;
using TestBase;
using TestCases;
using TestCases.AReferencedAssembly;
using Assert = TestBase.Assert;

namespace ActivateAnything.Specs.WhenTestBaseBuildsUsingRuleAttributes.ForTypeWithAbstractConstructorDependencies
{
	[FindInAssembliesInAppDomainBaseDirectory]
    public class GivenRule_FindInAssembliesInBaseDirectory : TestBaseFor<ClassWith1ConstructorParam<INterfaceWithClassInNotReferencedAssembly>>
    {
        [Fact]
        public void ThenI_FindConcreteTypeForInterfaceInAssembliesInBaseDirectory()
        {
            UnitUnderTest.ShouldNotBeNull();
            UnitUnderTest.ShouldBeAssignableTo<ClassWith1ConstructorParam<INterfaceWithClassInNotReferencedAssembly>>();
        }

        [Fact]
        public void ThenI_FindConcreteTypeEvenIfTheAssemblyIsntReferenced()
        {
            Assert.That(UnitUnderTest.Param1.GetType().Assembly.FullName.Contains("TestCases.ANotReferencedAssembly"));
        }
    }
}