using System.Collections.Generic;
using TestBase;
using Xunit;

namespace ActivateAnything.Specs.WhenBuildingAnInstance
{
    public class AARespectsChooseConstructorRules
    {
        [Fact]
        public void ChooseConstructorWithFewestParametersAttribute()
        {
            IEnumerable<IActivateAnythingRule> rules = new[] {new ConstructorWithFewestParametersRule()};
            //
            var result = new AnythingActivator(rules).New<ClassWithMultipleConstructors>();
            //
            result.param1.ShouldBeNull();
            result.param2.ShouldBeNull();
        }

        [Fact]
        public void ChooseConstructorWithMostParametersAttribute()
        {
            IEnumerable<IActivateAnythingRule> rules = new[] {new ConstructorWithMostParametersRule()};
            //
            var result = new AnythingActivator(rules).New<ClassWithMultipleConstructors>();
            //
            result.param1.ShouldNotBeNull();
            result.param2.ShouldNotBeNull();
        }
    }

    class ClassWithMultipleConstructors
    {
        public readonly string param1;
        public readonly string param2;

        public ClassWithMultipleConstructors() { }

        public ClassWithMultipleConstructors(string param1) { this.param1 = param1; }

        public ClassWithMultipleConstructors(string param1, string param2) : this(param1) { this.param2 = param2; }
    }
}
