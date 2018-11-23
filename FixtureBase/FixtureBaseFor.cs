using TestBase.AdoNet;
using TestBase.HttpClient.Fake;

// Example FixtureBases for a classes which depend on HttpClient and/or IDbConnection
// 
// The fixture base automatically creates a UnitUnderTest and replaces dependencies on IDbConnection with
// a FakeDbConnection and dependencies on a System.Net.Http.HttpClient with a TestBase.HttpClient.Fake.FakeHttpClient.
// 
// Even if the dependency on the database or HttpClient is an indirect dependency, two or more levels deep,
// they still get injected.
// 
// See https://github.com/chrisfcarroll/ActivateAnything/blob/master/FixtureBase/FixtureBase.cs for how it's done.
// 
namespace FixtureBase
{
    /// <inheritdoc cref="FixtureBaseFor{T}"/>
    public class FixtureBaseWithHttpFor<T> : FixtureBaseFor<T>
    {
        /// <summary>A <see cref="FakeHttpClient"/> which can be setup for expectations, return results, and be verified.
        /// This is added to <see cref="FixtureBase.Instances"/> for use by <see cref="FixtureBase.Activator"/> in
        /// constructing the <see cref="FixtureBaseFor{T}.UnitUnderTest"/>
        /// </summary>
        public readonly FakeHttpClient HttpClient = new FakeHttpClient();

        /// <inheritdoc />
        /// <inheritdoc />
        /// <summary>Create an instance of <see cref="FixtureBaseWithDbAndHttpFor{T}"/> with a <see cref="FakeHttpClient"/> 
        /// already added to <see cref="FixtureBase.Instances"/></summary>
        protected FixtureBaseWithHttpFor() {Instances.Add(HttpClient);}
    }

    
    /// <inheritdoc cref="FixtureBaseFor{T}"/>
    public class FixtureBaseWithDbFor<T> : FixtureBaseFor<T>
    {
        /// <summary>A <see cref="FakeDbConnection"/> which can be setup for expectations, return results, and be verified.
        /// This is added to <see cref="FixtureBase.Instances"/> for use by <see cref="FixtureBase.Activator"/> in
        /// constructing the <see cref="FixtureBaseFor{T}.UnitUnderTest"/>
        /// </summary>
        public readonly FakeDbConnection FakeDbConnection = new FakeDbConnection();

        /// <inheritdoc />
        /// <summary>Create an instance of <see cref="FixtureBaseWithDbAndHttpFor{T}"/> with a <see cref="FakeDbConnection"/>
        /// already added to <see cref="FixtureBase.Instances"/></summary>
        protected FixtureBaseWithDbFor(){Instances.Add(FakeDbConnection);}
    }

    
    /// <inheritdoc cref="FixtureBaseFor{T}"/>
    public class FixtureBaseWithDbAndHttpFor<T> :FixtureBaseFor<T>
    {
        /// <summary>A <see cref="FakeDbConnection"/> which can be setup for expectations, return results, and be verified.
        /// This is added to <see cref="FixtureBase.Instances"/> for use by <see cref="FixtureBase.Activator"/> in
        /// constructing the <see cref="FixtureBaseFor{T}.UnitUnderTest"/>
        /// </summary>
        public readonly FakeDbConnection Db = new FakeDbConnection();
        
        /// <summary>A <see cref="FakeHttpClient"/> which can be setup for expectations, return results, and be verified.
        /// This is added to <see cref="FixtureBase.Instances"/> for use by <see cref="FixtureBase.Activator"/> in
        /// constructing the <see cref="FixtureBaseFor{T}.UnitUnderTest"/>
        /// </summary>
        public readonly FakeHttpClient HttpClient = new FakeHttpClient();

        /// <inheritdoc />
        /// <summary>Create an instance of <see cref="FixtureBaseWithDbAndHttpFor{T}"/> with a <see cref="FakeDbConnection"/>
        /// and a <see cref="FakeHttpClient"/> already added to <see cref="FixtureBase.Instances"/></summary>
        protected FixtureBaseWithDbAndHttpFor(){Instances.Add(Db);Instances.Add(HttpClient);}
    }
}
