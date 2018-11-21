namespace TestCases
{
    public class ClassWithPrivateConstructor
    {
        ClassWithPrivateConstructor(){}
        internal static ClassWithPrivateConstructor New() => new ClassWithPrivateConstructor();
    }
}