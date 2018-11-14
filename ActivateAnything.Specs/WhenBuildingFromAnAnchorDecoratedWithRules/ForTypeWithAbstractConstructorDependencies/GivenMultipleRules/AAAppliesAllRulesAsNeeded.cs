using System;
using Xunit;
using TestBase;
using TestCases;
using TestCases.AReferencedAssembly;
using Assert = TestBase.Assert;

namespace ActivateAnything.Specs.WhenBuildingFromAnAnchorDecoratedWithRules.ForTypeWithAbstractConstructorDependencies.GivenMultipleRules
{
    [FindInAnyAssemblyReferencedByAssemblyContainingType]
    [FindInAnchorAssembly]
    [FindInDirectory]
    [CreateFromMock(typeof(ICloneable))]
    public class AAAppliesAllRulesAsNeeded : 
                    TestBaseFor
                        <ClassWith4ConstructorParams
                            <INterfaceWithClassInSameAssembly, 
                            INterfaceWithFakeInTestAssembly, 
                            INterfaceWithClassInNotReferencedAssembly, 
                            ICloneable>>
    {
        [Fact]
        public void AndBuildsRequestedType()
        {
            UnitUnderTest.ShouldNotBeNull();
            UnitUnderTest.ShouldBeAssignableTo
                            <ClassWith4ConstructorParams
                                <INterfaceWithClassInSameAssembly, 
                                INterfaceWithFakeInTestAssembly, 
                                INterfaceWithClassInNotReferencedAssembly, 
                                ICloneable>>();
        }

        [Fact]
        public void AndFindsConcreteTypeForINterfaceWithClassInSameAssembly()
        {
            UnitUnderTest.Param1.ShouldBeAssignableTo<INterfaceWithClassInSameAssembly>();
        }

        [Fact]
        public void AndFindsConcreteTypeForINterfaceWithFakeInTestAssembly()
        {
            UnitUnderTest.Param2.ShouldBeAssignableTo<INterfaceWithFakeInTestAssembly>();
            Assert.That(UnitUnderTest.Param2.GetType().Assembly, Is.EqualTo(this.GetType().Assembly));
        }

        [Fact]
        public void AndFindsConcreteTypeForInterfaceInAssembliesInBaseDirectoryEvenIfTheAssemblyIsntReferenced()
        {
            UnitUnderTest.Param3.ShouldBeAssignableTo<INterfaceWithClassInNotReferencedAssembly>();
            Assert.That(UnitUnderTest.Param3.GetType().Assembly.FullName.Contains("TestCases.ANotReferencedAssembly"));
        }

        [Fact]
        public void AndMocksAnInterface__AssumingThatAKnownMockingFrameworkAssemblyWasFindable()
        {
            UnitUnderTest.Param4.ShouldBeAssignableTo<ICloneable>();
            Assert.That(CreateFromMockAttribute.IsAKnownMock(UnitUnderTest.Param4));
        }

    }
}
