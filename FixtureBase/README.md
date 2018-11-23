FixtureBase
===========

**FixtureBase** cuts the cost of unit testing. Intelligent, rule-driven, autofaking fixtures get you straight to your test with 
all your dependencies setup and ready to go.

Don't spend hours writing code to mock a dozen dependencies, and more hours debugging it. Just write your test code, and let 
FixtureBase create the dependencies for you.

FixtureBase constructs your UnitUnderTest to test your codebase end-to-end, with external dependencies auto-faked and automatically 
injected in just the right place; even constructor dependencies that are several layers deep. 

You just write your tests:
```
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
    public void UUTreturnsDataFromDbQuerySingleColumn()
    {
        var dbData = new[] { "row1", "row2", "row3", "row4"};
        Db.SetUpForQuerySingleColumn(dbData);

        UnitUnderTest.FromDbStrings().ShouldEqualByValue(dbData);
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
}
```
The included examples demonstrate FixtureBases for applications which depend on
Ado.Net IDbConnections and on HttpClient network connections.

-   To create your own FixtureBase with your own preferred Fakes, see the
    examples at
    <https://github.com/chrisfcarroll/ActivateAnything/blob/master/FixtureBase/FixtureExample.cs.md>
-   For how it's done, see
    <https://github.com/chrisfcarroll/ActivateAnything/blob/master/FixtureBase/FixtureBase.cs>

Construction is done by
-   [ActivateAnything](https://www.nuget.org/packages/ActivateAnything)

Faking is done by
-   [TestBase.AdoNet](https://www.nuget.org/packages/TestBase.AdoNet)
-   [TestBase.HttpClient.Fake](https://www.nuget.org/packages/TestBase.HttpClient.Fake)

For more tools focussed on cutting the cost of unit testing, see also:
-   [TestBase](https://www.nuget.org/packages/TestBase)
-   [TestBase.AspNetCore.Mvc](https://www.nuget.org/packages/TestBase.AspNetCore.Mvc)
-   [TestBase-Mvc](https://www.nuget.org/packages/TestBase-Mvc)
-   [TestBase.AdoNet](https://www.nuget.org/packages/TestBase.AdoNet)
-   [TestBase.HttpClient.Fake](https://www.nuget.org/packages/TestBase.HttpClient.Fake)
-   [Serilog.Sinks.ListOfString](https://www.nuget.org/packages/Serilog.Sinks.Listofstring)
-   [Extensions.Logging.ListOfString](https://www.nuget.org/packages/Extensions.Logging.ListOfString)
