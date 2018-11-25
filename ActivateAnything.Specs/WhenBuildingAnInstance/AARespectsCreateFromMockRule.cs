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
            var moq = new FindInAssembly("Moq").FindTypeAssignableTo("Mock`1");
            //
            Assert.That(moq,
            Is.NotNull,
            "Didn't find a known mock framework (i.e. Moq) in Base Directory, can't test mocking.");
        }

        [Fact]
        public void ForAClass()
        {
            var result =
            new AnythingActivator(new CreateFromMock(typeof(INterface)))
            .New<ClassWith1ConstructorParam<INterface>>();
            //
            Assert.That(result.Param1, x => CreateFromMock.IsAKnownMock(x));
        }
    }
}
