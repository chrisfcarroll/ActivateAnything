using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ActivateAnything;
using Dapper;
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

    public class AUseCase
    {
        readonly IServiceRequiringDBandHttp service;

        public AUseCase(IServiceRequiringDBandHttp service)
        {
            this.service = service;
        }

        public List<string> FromDb() => service.GetFromDb();
        public async Task<string> FromHttpService() => await service.GetFromHttp();
    }

    public interface IServiceRequiringDBandHttp
    {
        List<string> GetFromDb();
        Task<string> GetFromHttp();
    }

    class AServiceRequiringDBandHttp : IServiceRequiringDBandHttp
    {
        readonly IDbConnection db;
        readonly System.Net.Http.HttpClient httpClient;

        public AServiceRequiringDBandHttp(IDbConnection db, System.Net.Http.HttpClient httpClient)
        {
            this.db = db;
            this.httpClient = httpClient;
        }

        public List<string> GetFromDb()
        {
            return db.Query<string>("Select * Fom AClass").ToList();
        }

        public async Task<string> GetFromHttp()
        {
            return await httpClient.GetStringAsync("http://localhost/string");
        }
    }
}
