using TestBase;
using TestCases;
using Xunit;

namespace ActivateAnything.Specs.WhenBuildingFromAnAnchorDecoratedWithRules.ForTypeWithConcreteConstructorDependencies
{
    public class GivenTargetTypeWithDependencyOnValueType : TestBaseFor<ClassWith1ConstructorParam<int>>
    {
        [Fact]
        public void AAInstantiatesIt()
        {
            UnitUnderTest.ShouldNotBeNull();
            UnitUnderTest.ShouldBeAssignableTo<ClassWith1ConstructorParam<int>>();
        }
    }

    public class GivenTargetTypeWithDependencyOnString : TestBaseFor<ClassWith1ConstructorParam<string>>
    {
        [Fact]
        public void AAInstantiatesIt()
        {
            UnitUnderTest.ShouldNotBeNull();
            UnitUnderTest.ShouldBeAssignableTo<ClassWith1ConstructorParam<string>>();
        }
    }

    public class GivenTargetTypeWithDependencyOnTypeWithItself1ConstructorDependency : TestBaseFor<
        ClassWith1ConstructorParam<ClassWith1ConstructorParam<string>>>
    {
        [Fact]
        public void AAInstantiatesIt()
        {
            UnitUnderTest.ShouldNotBeNull();
            UnitUnderTest.ShouldBeAssignableTo<ClassWith1ConstructorParam<ClassWith1ConstructorParam<string>>>();
        }
    }
}
