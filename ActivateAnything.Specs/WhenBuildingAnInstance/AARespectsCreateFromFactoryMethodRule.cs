using System;
using TestBase;
using Xunit;
using Assert = TestBase.Assert;

namespace ActivateAnything.Specs.WhenBuildingAnInstance
{
    public class AARespectsCreateFromFactoryMethodRule
    {
        class AClass
        {
            public readonly string Aparameter;
            public AClass(string aparameter) { Aparameter = aparameter; }
        }

        class AFactory
        {
            public AClass BuildMethod(string aparameter) { return new AClass(aparameter); }
        }

        [Fact]
        public void ButIfFactoryMethodIsMissing_ThenAAThrows_WithOrWithoutAnAnchor()
        {
            var rules = new[] {new CreateFromFactoryMethod(typeof(AClass), "BuildMethodNameWhichDoesntExist")};
            //
            Assert.Throws<InvalidOperationException>(
                                                     () => new AnythingActivator(rules,andUseDefaultRules:false).New<AClass>()
                                                    );
            Assert.Throws<InvalidOperationException>(
                                                     () => new AnythingActivator(
                                                                                 this, 
                                                                                 instances:null, 
                                                                                 rules:rules, 
                                                                                 andUseDefaultRules:false)
                                                    .New<AClass>()
                                                    );
        }

        [Fact]
        public void ButIfFactoryMethodIsMissing_ThenCreateFromFactoryMethodAttributeConstructorThrows()
        {
            Assert.Throws<InvalidOperationException>(() =>
                                                     {
                                                         new CreateFromFactoryMethod(typeof(AClass),
                                                                                     typeof(AFactory),
                                                                                     "BuildMethodNameWhichDoesntExist");
                                                     });
        }

        [Fact]
        public void ForAClass()
        {
            const string aCustomString = "ACustomString";
            var result =
            new AnythingActivator(
                                  DefaultRules.All
                                              .Union(new CreateFromFactoryMethod(
                                                                                 typeof(AClass),
                                                                                 typeof(AFactory),
                                                                                 "BuildMethod",
                                                                                 aCustomString))).New<AClass>();
            //
            result.Aparameter.ShouldBe(aCustomString);
        }
    }
}
