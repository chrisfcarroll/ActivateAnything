using Assert = TestBase.Assert;

namespace ActivateAnything.Specs.WhenBuildingAnInstance
{
    static class MockObjectShould
    {
        public static void ShouldBeAMock(object value)
        {
            Assert.That(value, v => CreateFromMockAttribute.IsAKnownMock(value));
        }
    }
}