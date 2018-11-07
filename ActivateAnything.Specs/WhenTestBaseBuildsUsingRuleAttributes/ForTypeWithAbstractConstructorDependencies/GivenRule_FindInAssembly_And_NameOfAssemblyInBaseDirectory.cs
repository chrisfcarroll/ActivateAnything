using Xunit;
using TestBase;
using TestCases;
using TestCases.AReferencedAssembly;

namespace ActivateAnything.Specs.WhenTestBaseBuildsUsingRuleAttributes.ForTypeWithAbstractConstructorDependencies
{
	[FindInAssembly("TestCases.ANotReferencedAssembly")]
    public class GivenRule_FindInAssembly_And_NameOfAssemblyInBaseDirectory : TestBaseFor<ClassWith1ConstructorParam<INterfaceWithClassInNotReferencedAssembly>>
    {
        [Fact]
        public void ThenI_FindConcreteTypeForInterfaceInNamedAssembly()
        {
            UnitUnderTest.ShouldNotBeNull();

            UnitUnderTest.ShouldBeAssignableTo<ClassWith1ConstructorParam<INterfaceWithClassInNotReferencedAssembly>>();

            UnitUnderTest
                    .Param1.GetType()
                        .Assembly.FullName
                            .ShouldStartWith("TestCases.ANotReferencedAssembly");
        }
    }
    
}