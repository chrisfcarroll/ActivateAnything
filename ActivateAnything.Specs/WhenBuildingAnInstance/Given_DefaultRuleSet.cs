using System;
using Xunit;
using TestBase;
using TestCases;
using TestCases.AReferencedAssembly;

namespace ActivateAnything.Specs.WhenBuildingAnInstance
{
    public class Given_DefaultRuleSet
    {
        [Theory]
        [InlineData(typeof (ClassWithDefaultConstructor))]
        [InlineData(typeof(ClassWith1ConstructorParam<ClassWithDefaultConstructor>))]
        [InlineData(typeof(ClassWith3ConstructorParams<INterfaceWithClassInSameAssembly, INterfaceWithFakeInTestAssembly, INterfaceWithClassInNotReferencedAssembly>))]
        public void ThenI_BuildInstanceOfRequestedType(Type type)
        {
            var result= CreateInstance.Of(type);
            //
            type.ShouldBe(result.GetType());
        }

        [Fact]
        public void ThenI_BuildInstanceOfRequestedType__AndIGetTheDependenciesRightToo()
        {
            ClassWith3ConstructorParams<INterfaceWithClassInSameAssembly, INterfaceWithFakeInTestAssembly, INterfaceWithClassInNotReferencedAssembly> 
                result = CreateInstance.Of<ClassWith3ConstructorParams
                                                <INterfaceWithClassInSameAssembly, 
                                                 INterfaceWithFakeInTestAssembly,
                                                 INterfaceWithClassInNotReferencedAssembly>>();

            result.ShouldNotBeNull();
            result.Param1.ShouldNotBeNull();
            result.Param2.ShouldNotBeNull();
            result.Param3.ShouldNotBeNull();
        }
    }
}
