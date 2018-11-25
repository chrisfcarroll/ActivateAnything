using TestBase;
using TestCases;
using TestCases.AReferencedAssembly;
using Xunit;
using Assert = TestBase.Assert;

namespace ActivateAnything.Specs.WhenBuildingFromAnAnchorDecoratedWithRules
{
    [DefaultRules]
    public class TheDefaultRulesetSufficesForTargetTypeWithAbstractDependencies :
    TestBaseFor<ClassWith3ConstructorParams<INterfaceWithClassInSameAssembly, INterfaceWithFakeInTestAssembly,
        INterfaceWithClassInNotReferencedAssembly>>
    {
        [Fact]
        public void AACreatesAnInstance()
        {
            UnitUnderTest
           .ShouldNotBeNull()
           .ShouldBeAssignableTo<ClassWith3ConstructorParams<INterfaceWithClassInSameAssembly,
                INterfaceWithFakeInTestAssembly, INterfaceWithClassInNotReferencedAssembly>>();
        }

        [Fact]
        public void AAFulfillsConstructorDependencyOnInterfaceInAssembliesInBaseDirectoryEvenIfTheAssemblyIsntReferenced()
        {
            UnitUnderTest.Param3.ShouldBeAssignableTo<INterfaceWithClassInNotReferencedAssembly>();
            Assert.That(UnitUnderTest.Param3.GetType().Assembly.FullName.Contains("TestCases.ANotReferencedAssembly"));
        }

        [Fact]
        public void AAFulfillsConstructorDependencyOnINterfaceWithClassInSameAssembly()
        {
            UnitUnderTest.Param1.ShouldBeAssignableTo<INterfaceWithClassInSameAssembly>();
        }

        [Fact]
        public void AAFulfillsConstructorDependencyOnINterfaceWithFakeInTestAssembly()
        {
            UnitUnderTest.Param2.ShouldBeAssignableTo<INterfaceWithFakeInTestAssembly>();
            Assert.That(UnitUnderTest.Param2.GetType().Assembly, Is.EqualTo(GetType().Assembly));
        }
    }
}
