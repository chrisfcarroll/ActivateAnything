namespace ActivateAnything.Specs
{
    /// <summary>
    /// An anchor class whose subclasses can be decorated with <see cref="IActivateAnythingRule"/>s
    /// to construct a UnitUnderTest.
    /// Also, a base class for TestFixtures / TestClasses.
    /// </summary>
    /// <typeparam name="T">The <see cref="System.Type"/> of the <see cref="UnitUnderTest"/></typeparam>
    public class TestBaseFor<T>
    {
        protected internal T UnitUnderTest;

        protected TestBaseFor()
        {
            UnitUnderTest = CreateInstance.Of<T>(this);
        }
    }
}
