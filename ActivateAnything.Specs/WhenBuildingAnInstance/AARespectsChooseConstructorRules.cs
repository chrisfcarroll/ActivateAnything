using System.Collections.Generic;
using Xunit;
using TestBase;

namespace ActivateAnything.Specs.WhenBuildingAnInstance
{
    public class AARespectsChooseConstructorRules
    {
        [Fact]
        public void ChooseConstructorWithFewestParametersAttribute()
        {
            IEnumerable<IActivateAnythingRule> rules = new[] {new ChooseConstructorWithFewestParametersAttribute()};
            //
            var result = CreateInstance.Of<ClassWithMultipleConstructors>(rules);
            //
            result.param1.ShouldBeNull();
            result.param2.ShouldBeNull();
        }
        [Fact]
        public void ChooseConstructorWithMostParametersAttribute()
        {
            IEnumerable<IActivateAnythingRule> rules = new[] { new ChooseConstructorWithMostParametersAttribute() };
            //
            var result = CreateInstance.Of<ClassWithMultipleConstructors>(rules);
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

        public ClassWithMultipleConstructors(string param1, string param2) : this(param1)
        {
            this.param2 = param2;
        }
    }
}
