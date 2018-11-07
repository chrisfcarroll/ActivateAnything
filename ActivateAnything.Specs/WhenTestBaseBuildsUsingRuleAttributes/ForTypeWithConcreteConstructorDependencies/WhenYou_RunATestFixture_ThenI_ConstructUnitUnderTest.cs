using Xunit;
using TestBase;
using TestCases;

namespace ActivateAnything.Specs.WhenTestBaseBuildsUsingRuleAttributes.ForTypeWithConcreteConstructorDependencies
{
    public class Given_DependencyOnValueType : TestBaseFor<ClassWith1ConstructorParam<int>>
    {
        [Fact]public void ForTypeWithDependencyOnValueType()
        {
            UnitUnderTest.ShouldNotBeNull();
            UnitUnderTest.ShouldBeAssignableTo<ClassWith1ConstructorParam<int>>();
        }
    }

    public class Given_DependencyOnString : TestBaseFor<ClassWith1ConstructorParam<string>>
    {
        [Fact]
        public void ForTypeWithDependencyOnString()
        {
            UnitUnderTest.ShouldNotBeNull();
            UnitUnderTest.ShouldBeAssignableTo<ClassWith1ConstructorParam<string>>();
        }
    }

    public class Given_DependencyOnTypeWithItself1ConstructorDependency : TestBaseFor<ClassWith1ConstructorParam<ClassWith1ConstructorParam<string>>>
    {
        [Fact]
        public void ForTypeWithDependencyOnTypeWithItself1ConstructorDependency()
        {
            UnitUnderTest.ShouldNotBeNull();
            UnitUnderTest.ShouldBeAssignableTo<ClassWith1ConstructorParam<ClassWith1ConstructorParam<string>>>();
        }
    }
}