using System;
using ActivateAnything;
using TestBase.AdoNet;
using TestBase.HttpClient.Fake;
using Xunit;

namespace TestBase.FixtureBase.Specs

{
    public class FixtureBaseSpecs : FixtureBaseWithDbAndHttpFor<AUseCase>
    {
        [Fact]
        public void UUTShouldNotBeNull()
        {
            try
            {
                UnitUnderTest.ShouldNotBeNull();
                Console.WriteLine("----------------");
                foreach (var kv in Activator.LastActivationTree)
                {
                    Console.WriteLine(kv.ToString(null));
                }
                Console.WriteLine("----------------");
                foreach (var kv in Activator.LastErrorList)
                {
                    Console.WriteLine( kv.Key.ToString(ActivationInfoFormat.TypeName) + " :: " + kv.Value);
                }
                Console.WriteLine("----------------");
            } 
            catch (Exception)
            {
                Console.WriteLine("----------------");
                foreach (var kv in Activator.LastErrorList)
                {
                    Console.WriteLine( kv.Key.ToString(ActivationInfoFormat.TypeName) + " :: " + kv.Value);
                }
                Console.WriteLine("----------------");
                foreach (var kv in Activator.LastActivationTree)
                {
                    Console.WriteLine(kv.ToString(null));
                }
                Console.WriteLine("----------------");
                throw;
            }
        }

        [Fact]public void DbShouldNotBeNull() => Db.ShouldNotBeNull();
        [Fact]public void HttpClientShouldNotBeNull() => HttpClient.ShouldNotBeNull();
        [Fact]public void DbShouldBeFakeDb() => Db.ShouldBeIn(Instances).ShouldBeOfType<FakeDbConnection>();
        [Fact]public void HttpClientShouldBeFakeHttpClient() => HttpClient.ShouldBeIn(Instances).ShouldBeOfType<FakeHttpClient>();
    }
}
