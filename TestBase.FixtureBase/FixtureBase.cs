using System.Collections.ObjectModel;
using System.Linq;
using ActivateAnything;
using TestBase.AdoNet;
using TestBase.HttpClient.Fake;

namespace TestBase.FixtureBase
{
    /// <summary>
    ///     An anchor class for <see cref="Activator"/> suitable as a testfixture, including <see cref="Rules"/> and
    ///     <see cref="Instances"/> and whose subclasses can also be decorated with <see cref="IActivateAnythingRule" /> attributes.
    ///
    /// <seealso cref="AnythingActivator"/>
    /// <seealso cref="IActivateAnythingRule"/>
    /// </summary>
    public class FixtureBase
    {
        readonly object aalocker=new object();
        bool activateIsStale = true;
        AnythingActivator activator;

        /// <summary>A collection of objects that you have manually constructed in preference to letting
        /// them be auto-constructed. Use this when it is simpler to construct an instance yourself than to specify rules for its construction.
        /// Items in this collection are used as a first preference by <see cref="Activator"/> whenever then can fulfill a dependency.
        ///
        /// <seealso cref="AnythingActivator"/>
        /// <seealso cref="IActivateAnythingRule"/>
        /// </summary>
        public ObservableCollection<object> Instances { get; } = new ObservableCollection<object>();
        
        /// <summary>Rules to be used by <see cref="Activator"/>, to be used in priority ahead of the default rules but after
        /// the use of <see cref="Instances"/>
        /// </summary>
        public ObservableCollection<IActivateAnythingRule> Rules { get; } = new ObservableCollection<IActivateAnythingRule>();
        
        /// <summary>An <see cref="AnythingActivator"/> which uses <see cref="this"/> as a <see cref="AnythingActivator.SearchAnchor"/>
        /// and uses the union of
        /// <list type="bullet">
        ///  <item><see cref="Instances"/></item>
        ///  <item><see cref="Rules"/></item>
        ///  <item>Any <see cref="IActivateAnythingRule"/>s decorating <c>this</c> as <c>Attributes</c></item>
        ///  <item><see cref="AnythingActivator.DefaultRules"/></item>
        /// </list>
        /// in that order.
        /// </summary>
        public AnythingActivator Activator
        {
            get
            {
                if (activateIsStale || activator==null)lock(aalocker)if (activateIsStale || activator==null)
                {
                    var rules =
                        Rules
                           .After(new ActivateInstances(Instances.ToArray()));
                    
                    activator = AnythingActivator.FromDefaultAndSearchAnchorRulesAnd(this, rules );
                    activateIsStale = false;
                }
                return activator;
            }
        }

        protected FixtureBase()
        {
            Instances.CollectionChanged += (sender, args) => activateIsStale = true;
            Rules.CollectionChanged += (sender, args) => activateIsStale = true;
        }
    }

    
    /// <summary>
    ///     An anchor class for <see cref="FixtureBase.Activator"/> suitable as a TestFixture base class
    ///     for testing a <see cref="UnitUnderTest"/> of Type <typeparam name="T"></typeparam>.
    ///     
    ///     The classes exposes <see cref="FixtureBase.Rules"/> and <see cref="FixtureBase.Instances"/> and   
    ///     can also be decorated with additional <see cref="IActivateAnythingRule" /> attributes.
    /// 
    /// <seealso cref="FixtureBase"/>
    /// <seealso cref="AnythingActivator"/>
    /// <seealso cref="IActivateAnythingRule"/>
    /// </summary>
    public class FixtureBaseFor<T> : FixtureBase
    {
        readonly object uutlocker=new object();
        bool uutIsStale = true;
        T uut;

        /// <summary>The unit under test, which will be auto-constructed by <see cref="FixtureBase.Activator"/>.
        /// Any constructor dependencies will be fulfilled by the Activator using:
        /// <list type="bullet">
        ///  <item><see cref="Instances"/></item>
        ///  <item><see cref="Rules"/></item>
        ///  <item>Any <see cref="IActivateAnythingRule"/>s decorating <c>this</c> as <c>Attributes</c></item>
        ///  <item><see cref="AnythingActivator.DefaultRules"/></item>
        /// </list>
        /// in that order.
        ///
        /// <seealso cref="AnythingActivator"/>
        /// <seealso cref="IActivateAnythingRule"/>
        /// </summary>
        public T UnitUnderTest
        {
            get
            {
                if (uutIsStale || uut==null)lock(uutlocker)if (uutIsStale || uut==null)
                {
                    uut = Activator.New<T>();
                    uutIsStale = false;
                }
                return uut;
            }
        }

        /// <inheritdoc />
        protected FixtureBaseFor()
        {
            Instances.CollectionChanged += (sender, args) => uutIsStale = true;
            Rules.CollectionChanged += (sender, args) => uutIsStale = true;
        }
    }
    
    /// <inheritdoc cref="FixtureBaseFor{T}"/>
    public class FixtureBaseWithHttpFor<T> : FixtureBaseFor<T>
    {
        /// <summary>A <see cref="FakeHttpClient"/> which can be setup for expectations, return results, and be verified.
        /// This is added to <see cref="FixtureBase.Instances"/> for use by <see cref="FixtureBase.Activator"/> in
        /// constructing the <see cref="FixtureBaseFor{T}.UnitUnderTest"/>
        /// </summary>
        public readonly FakeHttpClient HttpClient = new FakeHttpClient();

        /// <inheritdoc />
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
        protected FixtureBaseWithDbAndHttpFor(){Instances.Add(Db);Instances.Add(HttpClient);}
    }
}
