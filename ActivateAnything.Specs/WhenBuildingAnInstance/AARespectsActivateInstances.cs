using System;
using System.Linq;
using System.Linq.Expressions;
using TestBase;
using Xunit;
using Xunit.Abstractions;
using Assert = TestBase.Assert;

namespace ActivateAnything.Specs.WhenBuildingAnInstance
{
    public class AARespectsActivateInstances
    {
        readonly ITestOutputHelper xconsole;
        public AARespectsActivateInstances(ITestOutputHelper xconsole) { this.xconsole = xconsole; }

        class AClass{}

        [Fact]
        public void ForAClass()
        {
            var myObject = new AClass();
            var result =
                new AnythingActivator(AnythingActivator.DefaultRules.After(new ActivateInstances(myObject)))
                    .New<AClass>();
            //
            Assert.That(myObject == result);
        }


        [Fact]
        public void ForAString()
        {
            var result =
                new AnythingActivator(
                        ActivateDefaultRules.AllDefaultRules
                            .After(
                                new[] {new ActivateInstances("ACustomString"), }))
                    .New<string>();
            //
            result.ShouldBe("ACustomString");
        }
        
        [Fact]
        public void ForAFuncOf()
        {
            var myObject = new AClass();
            var myString = "String!";            
            
            //
            var uut = new AnythingActivator(AnythingActivator.DefaultRules.After(new ActivateInstances(myObject,myString)));            
            var result1 =uut.New<Func<AClass>>();
            var result2 = uut.New<Func<string>>();
            
            //Debug
            xconsole.WriteLine(string.Join(Environment.NewLine, uut.LastErrorList));
            xconsole.WriteLine(string.Join(Environment.NewLine, uut.LastActivationTree));
            
            //
            Assert.That(result1, x=>x()==myObject);
            Assert.That(result2, x=>x() ==myString);
        }
    }
}