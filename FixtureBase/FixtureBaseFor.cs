using TestBase.AdoNet;
using TestBase.HttpClient.Fake;

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
