using TestBase;
using Xunit;

namespace ActivateAnything.Specs.WhenBuildingFromAnAnchorDecoratedWithRules
{
    [ChooseExactType(typeof(IWrappable), typeof(WrappableBackstop))]
    public class TheAnchorIsInspectedForDecoration
    {
        [Fact]
        public void AndAABuildsRequestedType()
        {
            var activator = new AnythingActivator(this);
            var wrappable = activator.New<IWrappable>();
            wrappable.ShouldNotBeNull()
                     .ShouldBeAssignableTo<IWrappable>()
                     .ShouldBeOfType<WrappableBackstop>();
        }
    }
}
