using System;
using System.Linq;
using TestBase;
using Xunit;

namespace ActivateAnything.Specs
{
    // WIP this test can't currently pass
    // Have to re-do how circular dependencies are avoided to 
    // allow pipelines to be built.
    // public class TheDefaultRulesetSufficesForSelfWrappingTypes
    // {
    //     
    //     [Fact]
    //     public void AndAABuildsRequestedType()
    //     {
    //         var activator     = new AnythingActivator(this);
    //         var UnitUnderTest = activator.New<IWrappable>();
    //         UnitUnderTest
    //                        .ShouldNotBeNull()
    //                        .ShouldBeAssignableTo<IWrappable>();
    //     }
    //     
    //     [Fact]
    //     public void AndAAChoosesShortestConstructor()
    //     {
    //         var activator     = new AnythingActivator(this);
    //         var UnitUnderTest = activator.New<IWrappable>();
    //         UnitUnderTest.ShouldBeOfType <WrappableBackstop>();
    //     }
    // }
    
    public interface IWrappable
    {
        Type[] WhoAmI();
    }

    public class Wrapper : IWrappable
    {
        internal IWrappable inner;
        public Wrapper(IWrappable inner) { this.inner = inner; }
        public Type[] WhoAmI() { return new[] { GetType() }.Union(inner.WhoAmI()).ToArray(); }
    }

    public class WrappableBackstop : IWrappable
    {
        public Type[] WhoAmI() { return new[] { GetType() }; }
    }
}
