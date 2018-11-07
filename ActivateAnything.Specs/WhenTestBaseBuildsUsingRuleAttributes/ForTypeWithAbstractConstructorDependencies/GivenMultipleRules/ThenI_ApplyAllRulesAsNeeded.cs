using System;
using Xunit;
using TestBase;
using ActivateAnything.Specs.WhenBuildingAnInstance;
using TestCases;
using TestCases.AReferencedAssembly;
using Assert = TestBase.Assert;

namespace ActivateAnything.Specs.WhenTestBaseBuildsUsingRuleAttributes.ForTypeWithAbstractConstructorDependencies.GivenMultipleRules
{
    [FindInAnyAssemblyReferencedByAssemblyContainingType]
    [FindOnlyInAnchorAssembly]
    [FindInAssembliesInAppDomainBaseDirectory]
    [CreateFromMock(typeof(ICloneable))]
    public class ThenI_ApplyAllRulesAsNeeded : TestBaseFor<ClassWith4ConstructorParams<INterfaceWithClassInSameAssembly, INterfaceWithFakeInTestAssembly, INterfaceWithClassInNotReferencedAssembly, ICloneable>>
    {
        [Fact]
        public void AndI_BuildRequestedType()
        {
            UnitUnderTest.ShouldNotBeNull();
            UnitUnderTest.ShouldBeAssignableTo<ClassWith4ConstructorParams<INterfaceWithClassInSameAssembly, INterfaceWithFakeInTestAssembly, INterfaceWithClassInNotReferencedAssembly, ICloneable>>();
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

        [Fact(Skip = WIP.Next)]
        public void AndI_MockAnInterface__AssumingThatAKnownMockingFrameworkAssemblyWasFindable()
        {
            UnitUnderTest.Param4.ShouldBeAssignableTo<ICloneable>();
            Assert.That(MockHelper.IsAMock(UnitUnderTest.Param4));
        }

    }
}
