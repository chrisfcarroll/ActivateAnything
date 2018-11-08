using System;
using System.Linq;
using TestBase;
using TestCases;
using Xunit;
using Assert = TestBase.Assert;

namespace ActivateAnything.Specs.WhenBuildingAnInstance
{
    public class AARespectsCreateFromMockRule
    {
        [TestTimeDependency("Moq.dll required in base directory")]
        public void ThereIsAMockingFrameworkInTheBaseDirectory()
        {
            var moq= new FindInAssemblyAttribute("Moq").FindTypeAssignableTo("Mock`1");
            //
            Assert.That(moq, Is.NotNull, "Didn't find a known mock framework (i.e. Moq) in Base Directory, can't test mocking.");
        }

        [Fact(Skip = WIP.Next)]
        public void ForAClass()
        {
            var result =
                CreateInstance
                    .Of<ClassWith1ConstructorParam<INterface>>(
                        new[] { new CreateFromMockAttribute(typeof(INterface)) }
                        );
            //
            Assert.That(result.Param1, x=>MockHelper.IsAMock(x));
        }
    }


    static class MockHelper
    {
        public static void ShouldBeAMock(object value) { knownIsMockTests.ShouldContain(m => m(value)); }
        public static bool IsAMock(object value) { return knownIsMockTests.Select(m => m(value)).Any(); }

        static Func<object, bool>[] knownIsMockTests = { MoqMocker.Instance.IsThisMyMockObject };
    }
}