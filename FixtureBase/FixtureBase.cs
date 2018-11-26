using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ActivateAnything;

namespace FixtureBase
{
    /// <summary>
    ///     An anchor class for <see cref="Activator" /> suitable as a testfixture, including <see cref="Rules" /> and
    ///     <see cref="Instances" /> and whose subclasses can also be decorated with <see cref="IActivateAnythingRule" />
    ///     attributes.
    ///     <seealso cref="AnythingActivator" />
    ///     <seealso cref="IActivateAnythingRule" />
    /// </summary>
    public class FixtureBase
    {
        readonly object aalocker = new object();
        AnythingActivator activator;

        /// <summary>
        ///     A reference to <see cref="AnythingActivator.Instances"/>
        ///     <seealso cref="AnythingActivator" />
        ///     <seealso cref="IActivateInstanceRule" />
        /// </summary>
        public List<object> Instances => Activator.Instances;

        /// <summary>
        ///     A reference to <see cref="AnythingActivator.Rules" />
        ///     <seealso cref="AnythingActivator" />
        ///     <seealso cref="IActivateAnythingRule" />
        /// </summary>
        public List<IActivateAnythingRule> Rules => Activator.Rules;

        /// <summary>
        ///     An <see cref="AnythingActivator" /> which uses <c>this</c> as a <see cref="AnythingActivator.SearchAnchor" />
        /// </summary>
        public AnythingActivator Activator
        {
            get
            {
                if (activator == null)lock (aalocker)if (activator == null) { activator = new AnythingActivator(this); }
                return activator;
            }
        }
    }

    /// <summary>
    ///     An anchor class for <see cref="FixtureBase.Activator" /> suitable as a TestFixture base class
    ///     for testing a <see cref="UnitUnderTest" /> of Type
    ///     <typeparam name="T"></typeparam>
    ///     .
    ///     The classes exposes <see cref="FixtureBase.Rules" /> and <see cref="FixtureBase.Instances" /> and
    ///     can also be decorated with additional <see cref="IActivateAnythingRule" /> attributes.
    ///     <seealso cref="FixtureBase" />
    ///     <seealso cref="AnythingActivator" />
    ///     <seealso cref="IActivateAnythingRule" />
    /// </summary>
    public class FixtureBaseFor<T> : FixtureBase
    {
        readonly object uutlocker = new object();
        T uut;

        /// <summary>
        ///     The unit under test, which will be auto-constructed by <see cref="FixtureBase.Activator" />.
        ///     Any constructor dependencies will be fulfilled by the Activator using:
        ///     <list type="bullet">
        ///         <item>
        ///             <see cref="FixtureBase.Instances" />
        ///         </item>
        ///         <item>
        ///             <see cref="FixtureBase.Rules" />
        ///         </item>
        ///         <item>Any <see cref="IActivateAnythingRule" />s decorating <c>this</c> as <c>Attributes</c></item>
        ///         <item>
        ///             <see cref="DefaultRules.All" />
        ///         </item>
        ///     </list>
        ///     in that order.
        ///     <seealso cref="AnythingActivator" />
        ///     <seealso cref="IActivateAnythingRule" />
        /// </summary>
        public T UnitUnderTest
        {
            get
            {
                if ( uut == null)lock(uutlocker)if(uut == null)
                {
                    uut = Activator.New<T>();
                }
                return uut;
            }
            set { uut = value; }
        }
    }
}
