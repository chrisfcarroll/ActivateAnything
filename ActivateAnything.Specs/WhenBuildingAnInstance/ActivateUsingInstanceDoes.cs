using TestBase;
using Xunit;

namespace ActivateAnything.Specs.WhenBuildingAnInstance
{
    public class ActivateUsingInstanceUsesTheInstance
    {
        static IWrappable UseMe = new Wrapper(new Wrapper(new WrappableBackstop()));
        
        [Fact]
        public void UseGivenInstance()
        {
            var rule = new ActivateUsingInstance(
                                                 typeof(IWrappable),
                                                 typeof(ActivateUsingInstanceUsesTheInstance),
                                                 nameof(UseMe));
            Activate.FromDefaultRulesAnd<IWrappable>(this,rule);
        }
    }
}
