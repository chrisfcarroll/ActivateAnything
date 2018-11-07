using Xunit;
using TestBase;
using TestCases;
using TestCases.AReferencedAssembly;
using Assert = TestBase.Assert;

namespace ActivateAnything.Specs.WhenTestBaseBuildsUsingRuleAttributes
{
	[ActivateAnythingDefaultRules]
    public class Given_DefaultRuleset__ForTestCaseWith3AbstractDependencies :
        TestBaseFor<ClassWith3ConstructorParams<INterfaceWithClassInSameAssembly, INterfaceWithFakeInTestAssembly, INterfaceWithClassInNotReferencedAssembly>>
    {
        [Fact]
        public void AndI_BuildRequestedType()
        {
            UnitUnderTest
                .ShouldNotBeNull()
                .ShouldBeAssignableTo<ClassWith3ConstructorParams<INterfaceWithClassInSameAssembly, INterfaceWithFakeInTestAssembly, INterfaceWithClassInNotReferencedAssembly>>();
        }

        [Fact]
        public void AndI_FindConcreteTypeForINterfaceWithClassInSameAssembly()
        {
            UnitUnderTest.Param1.ShouldBeAssignableTo<INterfaceWithClassInSameAssembly>();
        }

        [Fact]
        public void AndI_FindConcreteTypeForINterfaceWithFakeInTestAssembly()
        {
            UnitUnderTest.Param2.ShouldBeAssignableTo<INterfaceWithFakeInTestAssembly>();
            Assert.That(UnitUnderTest.Param2.GetType().Assembly, Is.EqualTo(this.GetType().Assembly));
        }

        [Fact]
        public void AndI_FindConcreteTypeForInterfaceInAssembliesInBaseDirectoryEvenIfTheAssemblyIsntReferenced()
        {
            UnitUnderTest.Param3.ShouldBeAssignableTo<INterfaceWithClassInNotReferencedAssembly>();
            Assert.That(UnitUnderTest.Param3.GetType().Assembly.FullName.Contains("TestCases.ANotReferencedAssembly"));
        }
    }
}