using TestBase;

namespace ActivateAnything.Specs.WhenBuildingAnInstance
{
    static class MockObjectShould
    {
        public static void ShouldBeAMock(object value) { Assert.That(value, v => CreateFromMock.IsAKnownMock(value)); }
    }
}
