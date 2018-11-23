/// To create a FixtureBase with your own preferred Fakes, see the examples at 
/// [https://github.com/chrisfcarroll/ActivateAnything/blob/master/FixtureBase/FixtureBaseFor.cs](https://github.com/chrisfcarroll/ActivateAnything/blob/master/FixtureBase/FixtureBaseFor.cs)

```
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dapper;
using TestBase;
using TestBase.AdoNet;
using Xunit;
using Xunit.Abstractions;

namespace FixtureBase.Specs
{
    /// <summary>
    /// An example FixtureBase for a class which depends on a Database and an HttpClient.
    /// 
    /// The fixture base automatically creates a UnitUnderTest and replaces dependencies on 
    /// <see cref="IDbConnection"/> with a <see cref="FakeDbConnection"/> and dependencies on a 
    /// <see cref="System.Net.Http.HttpClient"/> with a <see cref="TestBase.HttpClient.Fake.FakeHttpClient"/>
    /// 
    /// Even if the dependency on the database or HttpClient is an indirect dependency, two or more levels deep,
    /// they still get injected.
    /// 
    /// </summary>
    public class FixtureBaseExample : FixtureBaseWithDbAndHttpFor<AUseCase>
    {
        [Fact]
        public void UUTSendsDataToDb()
        {
            var newDatum = new Datum{Id=99, Name="New!" };

            UnitUnderTest.InsertDb(newDatum);

            Db.ShouldHaveInserted("Data",newDatum);
        }

        [Fact]
        public void UUTreturnsDataFromDbQueryScalar()
        {
            Db.SetUpForQueryScalar(999);

            UnitUnderTest.FromDb().ShouldBeOfLength(1).First().ShouldBe(999);
        }

        [Fact]
        public void UUTreturnsDataFromDbQuery()
        {
            var dataToReturn = new[]
            {
                new Datum {Id = 11, Name = "cell 1,2"}, 
                new Datum {Id = 21, Name = "cell 2,2"}
            };
            Db.SetUpForQuery(dataToReturn,new[] {"Id", "Name"});

            UnitUnderTest
                .FromDbIdAndNames()
                .ShouldEqualByValue(dataToReturn);
        }

        [Fact]
        public async Task UUTGetHttpReturnsDataFromService()
        {
            var contentFromService = "IGotThis!";
            HttpClient
                .Setup(m => true)
                .Returns(new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(contentFromService)});

            (await UnitUnderTest.GetHttp()).ShouldBe(contentFromService);

            HttpClient.Verify(x=>x.Method==HttpMethod.Get);
        }

        [Fact]
        public async Task UUTPutSendsDataToService()
        {
            HttpClient.Setup(m=>true).Returns(new HttpResponseMessage(HttpStatusCode.OK));

            var result= await UnitUnderTest.PutHttp("Put!");
            console.WriteLine(result.ToJSon());

            HttpClient.Verify(x=>x.Method==HttpMethod.Put && x.Content.EqualsByValue(new StringContent("Put!")));
        }

        [Fact]
        public void UUTreturnsDataFromDbQuerySingleColumn()
        {
            var dbData = new[] { "row1", "row2", "row3", "row4"};
            Db.SetUpForQuerySingleColumn(dbData);

            UnitUnderTest.FromDbStrings().ShouldEqualByValue(dbData);
        }

        public FixtureBaseExample(ITestOutputHelper console) => this.console = console;
        readonly ITestOutputHelper console;
    }


    // ------------------------------------------------------------------------------------ //


    public class Datum{ public int Id { get;set; } public string Name { get; set; }}

    public class AUseCase
    {
        readonly IServiceRequiringDBandHttp service;

        public AUseCase(IServiceRequiringDBandHttp service)
        {
            this.service = service;
        }

        public async Task<bool> PutHttp(string name) => await service.PutHttp(name);
        public async Task<string> GetHttp() => await service.GetHttpAsync();
        public List<int> FromDb() => service.GetFromDb();
        public List<Datum> FromDbIdAndNames() => service.GetFromDbIdAndNames();
        public List<string> FromDbStrings() => service.GetFromDbStrings();
        internal bool InsertDb(Datum newDatum) => service.InsertDb(newDatum);
    }

    public interface IServiceRequiringDBandHttp
    {
        Task<bool> PutHttp(string name);
        Task<string> GetHttpAsync();
        List<Datum> GetFromDbIdAndNames();
        List<int> GetFromDb();
        List<string> GetFromDbStrings();
        bool InsertDb(Datum newDatum);
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

        public async Task<string> GetHttpAsync(){ return await httpClient.GetStringAsync("http://localhost/things") ;}

        public async Task<bool> PutHttp(string name)
        { 
            var result= await httpClient.PutAsync("http://localhost/things", new StringContent($"{{\"Name\"=\"{name}\"}}"));
            result.EnsureSuccessStatusCode();
            return result.IsSuccessStatusCode;
        }

        public List<Datum> GetFromDbIdAndNames(){ return db.Query<Datum>("Select Id,Name Fom AClass").ToList(); }

        public List<int> GetFromDb(){ return db.Query<int>("Select Id Fom AClass").ToList(); }

        public List<string> GetFromDbStrings(){ return db.Query<string>("Select Name Fom AClass").ToList(); }

        public bool InsertDb(Datum newDatum){ return db.Execute("Insert into Data (Id,Name) Values (@Id,@Name)",newDatum)>0; }
    }
}
```