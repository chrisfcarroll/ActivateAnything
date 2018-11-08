using System;
using System.IO;
using System.Reflection;

namespace ActivateAnything
{
    public class MoqMocker : IMockingLibraryAdapter, IMockingLibraryAdapterWithInspections
    {
        class FindMoqMock : FindInAssemblyAttribute
        {
            public FindMoqMock() : base("Moq", t=> !t.ContainsGenericParameters){}
        }

        public static MoqMocker Instance = new MoqMocker();
        readonly object mockerTypeLocker = new object();
        readonly Func<Type,Type> MoqMakeMock;
        readonly MethodInfo MoqMockGetMethod;

        protected MoqMocker()
        {
            if(MoqMakeMock== null)lock(mockerTypeLocker)
            {
                var finder1 = new FindInAssemblyAttribute("Moq");
                MoqMakeMock = t => finder1.FindTypeAssignableTo("Moq.Mock`1").MakeGenericType(t);
                var finder2 = new FindMoqMock();
                var moqMockType  = finder2.FindTypeAssignableTo("Moq.Mock");
                MoqMockGetMethod = moqMockType.GetMethod("Get", BindingFlags.Public | BindingFlags.Static);
            }
            EnsureMockingAssemblyIsLoadedAndWorkingElseThrow();
        }

        public object CreateMockElseNull(Type type, params object[] mockConstructorArgs)
        {
            try { return CreateMockElseThrow(type, mockConstructorArgs); }catch{ return null; }
        }

        public object CreateMockElseThrow(Type type, params object[] mockConstructorArgs)
        {
            try
            {
                var mockedType = 
                    MoqMakeMock(type)
                    .Ensure(t => t != null, "Failed to make the Moq<T> GenericType needed to mock T. Just got null.");
                var mock = Activator
                    .CreateInstance(mockedType, mockConstructorArgs)
                    .EnsureNotNull(string.Format("Activator.CreateInstance({0},{1}) failed, just got null",
                                                 mockedType.Name,
                                                 mockConstructorArgs));

                var mockedObjectProperty = mockedType
                    .GetProperty("Object", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
                    .EnsureNotNull(string.Format("Reflected call to Moq<{0}>.GetProperty(\"Object\") failed, just got null.", type.FullName));

                return mockedObjectProperty.GetValue(mock,null).EnsureNotNull("Reflected call to Moq<{0}>.Object failed, just got null.");
            }
            catch (Exception e)
            {
                throw new Exception( $"{typeof(MoqMocker).FullName} failed to create a Moq<{type.FullName}>", e);
            }
        }


        public void EnsureMockingAssemblyIsLoadedAndWorkingElseThrow()
        {
            if(!IsMockingAssemblyFound())
            {
                throw new FileNotFoundException(
                    string.Format("Unable to find a Moq.dll with a Moq.Mock`1 Type in BaseDirectory {0}. ", AppDomain.CurrentDomain.BaseDirectory) +
                    "Moq is most easily added a NuGet dependency to it from your Test project. ",
                    "Moq.dll");
            }
            CreateMockElseThrow(typeof(ICloneable /*an arbitrary example type that, if all is well, we will successfully mock.*/ ));
        }

        public bool IsMockingAssemblyFound() { return MoqMockGetMethod != null; }

        public bool IsThisMyMockObject(object value) => GetMock(value) != null;

        public object GetMock(object value)
        {
            Func<object,object> moqMockGetMethod = obj => MoqMockGetMethod.Invoke(null, new[] { obj });
            return moqMockGetMethod(value);
        }
    }
}