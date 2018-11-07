namespace ActivateAnything.Specs
{
    /// <summary>
    /// A Base class for TestFixtures / TestClasses which can create a UnitUnderTest
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
