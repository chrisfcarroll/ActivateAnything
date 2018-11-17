﻿using System;
using TestBase;
using TestCases;
using TestCases.AReferencedAssembly;
using Xunit;

namespace ActivateAnything.Specs.WhenBuildingAnInstance
{
    public class TheDefaultRulesetSufficesToBuild
    {
        [Theory]
        [InlineData(typeof(ClassWithDefaultConstructor))]
        [InlineData(typeof(ClassWith1ConstructorParam<ClassWithDefaultConstructor>))]
        [InlineData(
            typeof(ClassWith3ConstructorParams<INterfaceWithClassInSameAssembly, INterfaceWithFakeInTestAssembly,
                INterfaceWithClassInNotReferencedAssembly>))]
        public void AType(Type type)
        {
            var result = AnythingActivator.Instance.Of(type);
            //
            type.ShouldBe(result.GetType());
        }

        [Fact]
        public void AndToGetConstructorDependenciesRight()
        {
            var
                result = AnythingActivator.Instance.Of<ClassWith3ConstructorParams
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