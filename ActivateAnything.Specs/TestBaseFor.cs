namespace ActivateAnything.Specs
{
    /// <summary>
    ///     An anchor class suitable as a testfixture, whose subclasses can be decorated
    ///     with <see cref="IActivateAnythingRule" />s to construct a UnitUnderTest.
    /// </summary>
    /// <typeparam name="T">The <see cref="System.Type" /> of the <see cref="UnitUnderTest" /></typeparam>
    public class TestBaseFor<T>
    {
        protected readonly AnythingActivator Activator;
        protected internal T UnitUnderTest;

        protected TestBaseFor()
        {
            Activator     = new AnythingActivator(this);
            UnitUnderTest = Activator.New<T>();
        }

        protected TestBaseFor(params object[] useInstances)
        {
            var instances = new ActivateInstances(useInstances);
            var rules = DefaultRules.All
                                    .After(this.GetType().GetActivateAnythingRuleAttributes())
                                    .After(instances);
            Activator = new AnythingActivator(this,rules);
        }
    }
}
