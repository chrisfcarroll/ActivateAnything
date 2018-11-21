using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TestBase.AdoNet;
using Xunit;

namespace TestBase.FixtureBase.Specs

{
    public class FixtureBaseExample : FixtureBaseWithDbAndHttpFor<AUseCase>
    {
        [Fact]
        public void UUTreturnsDataFromDb()
        {
            var dbData = new[] { (1,"row1"), (2,"row2"), (3,"row3")};
            Db.SetUpForQuery(dbData);
            UnitUnderTest.FromDb().ShouldEqualByValue(dbData);
        }

        [Fact]
        public async Task UUTreturnsDataFromService()
        {
            var fromService = "FromService";
            HttpClient
                .Setup(m => true)
                .Returns(new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(fromService)});

            var result= await UnitUnderTest.FromHttpService();
            result.ShouldBe(fromService);
        }

    }
}
