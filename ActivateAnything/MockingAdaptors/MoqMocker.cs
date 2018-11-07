using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace ActivateAnything
{
    public class MoqMocker : IMockingLibraryAdapter, IMockingLibraryAdapterWithInspections
    {
        public object CreateMockElseNull(Type type, params object[] mockConstructorArgs)
        {
            try { return CreateMockElseThrow(type, mockConstructorArgs); }catch{ return null; }
        }

        public object CreateMockElseThrow(Type type, params object[] mockConstructorArgs)
        {
            try
            {
                mockType = MockerType
                    .MakeGenericType(type)
                    .Ensure(t => t != null, "Failed to make the Moq<T> GenericType needed to mock T. Just got null.");
                var mock = Activator
                    .CreateInstance(mockType, mockConstructorArgs)
                    .EnsureNotNull(string.Format("Activator.CreateInstance({0},{1}) failed, just got null",
                                                 mockType.Name,
                                                 mockConstructorArgs));

                var mockedObjectProperty = mockType
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

        public bool IsMockingAssemblyFound() { return MockerType != null; }

        public bool IsThisMyMockObject(object value) { return GetMock(value) != null; }

        public object GetMock(object value)
        {
            EnsureMockingAssemblyIsLoadedAndWorkingElseThrow();
            return MockerType.GetMethod("Get",BindingFlags.Public|BindingFlags.Static).Invoke(null, new[] { value });
        }

        public static Type MockerType { get { return mockerType = mockerType ?? new FindInAssemblyAttribute("Moq").FindTypeAssignableTo("Moq.Mock`1"); } }

        static Type mockerType;
        static Type mockType;
    }
}