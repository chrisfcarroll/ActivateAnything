using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ActivateAnything
{
    /// <inheritdoc cref="IMockingAdapter" />
    /// <inheritdoc cref="IMockingAdapterInspections" />
    public class MoqMocker : IMockingAdapter, IMockingAdapterInspections
    {
        /// <summary>A static instance.</summary>
        public static MoqMocker Instance = new MoqMocker();

        static readonly Regex IsTypeMoqMockRegex = new Regex(@"^Moq\.\S*Proxy, Moq");
        readonly object mockerTypeLocker = new object();
        readonly Func<Type, Type> MoqMakeMockType;

        /// <inheritdoc />
        protected MoqMocker()
        {
            if (MoqMakeMockType == null)
                lock (mockerTypeLocker)
                {
                    var finder1 = new FindInAssembly("Moq");
                    MoqMakeMockType = t => finder1.FindTypeAssignableTo("Moq.Mock`1").MakeGenericType(t);
                }

            EnsureMockingAssemblyIsLoadedAndWorkingElseThrow();
        }

        /// <inheritdoc />
        public object CreateMockElseNull(Type type, params object[] mockConstructorArgs)
        {
            try { return CreateMockElseThrow(type, mockConstructorArgs); } catch { return null; }
        }

        /// <inheritdoc />
        public object CreateMockElseThrow(Type type, params object[] mockConstructorArgs)
        {
            try
            {
                var mockedType =
                MoqMakeMockType(type)
               .Ensure(t => t != null, "Failed to make the Moq<T> GenericType needed to mock T. Just got null.");
                var mock = Activator
                          .CreateInstance(mockedType, mockConstructorArgs)
                          .EnsureNotNull(string.Format("Activator.CreateInstance({0},{1}) failed, just got null",
                                                       mockedType.Name,
                                                       mockConstructorArgs));

                var mockedObjectProperty = mockedType
                                          .GetProperty("Object",
                                                       BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
                                          .EnsureNotNull(string
                                                        .Format("Reflected call to Moq<{0}>.GetProperty(\"Object\") failed, just got null.",
                                                                type.FullName));

                return mockedObjectProperty.GetValue(mock, null)
                                           .EnsureNotNull("Reflected call to Moq<{0}>.Object failed, just got null.");
            } catch (Exception e)
            {
                throw new Exception($"{typeof(MoqMocker).FullName} failed to create a Moq<{type.FullName}>", e);
            }
        }


        /// <inheritdoc />
        public void EnsureMockingAssemblyIsLoadedAndWorkingElseThrow()
        {
            if (!IsMockingAssemblyFound())
                throw new FileNotFoundException(
                                                string
                                               .Format("Unable to find a Moq.dll with a Moq.Mock`1 Type in BaseDirectory {0}. ",
                                                       AppDomain.CurrentDomain.BaseDirectory)
                                              + "Moq is most easily added a NuGet dependency to it from your Test project. ",
                                                "Moq.dll");
            CreateMockElseThrow(typeof(ICloneable /*an arbitrary example type that, if all is well, we will successfully mock.*/
                                ));
        }

        /// <inheritdoc />
        public bool IsMockingAssemblyFound() { return MoqMakeMockType != null; }

        /// <inheritdoc />
        public bool IsThisMyMockObject(object value) { return GetMock(value) != null; }

        /// <inheritdoc />
        public object GetMock(object value)
        {
            var baseType = value.GetType().BaseType;
            return baseType != null && IsTypeMoqMockRegex.IsMatch(baseType.AssemblyQualifiedName ?? "");
        }

        class FindMoqMock : FindInAssembly
        {
            public FindMoqMock() : base("Moq", t => !t.ContainsGenericParameters) { }
        }
    }
}
