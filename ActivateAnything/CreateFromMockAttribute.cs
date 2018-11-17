using System;
using System.Collections.Generic;
using System.Linq;

namespace ActivateAnything
{
    public class CreateFromMockAttribute : Attribute, IActivateInstanceRule
    {
        public static List<IMockingLibraryAdapter> KnownMockingLibraryAdapters
            = new List<IMockingLibraryAdapter> {MoqMocker.Instance};

        readonly object[] mockConstructorArgs;

        readonly Type[] typesToMock;

        public CreateFromMockAttribute(Type typeToMock, params object[] mockConstructorArgs)
        {
            this.mockConstructorArgs = mockConstructorArgs;
            typesToMock = new[] {typeToMock};
        }

        public CreateFromMockAttribute(params Type[] typesToMock)
        {
            this.typesToMock = typesToMock;
            mockConstructorArgs = new object[0];
        }

        /// <summary>
        ///     Note that using this overload means that the same list of <paramref name="mockConstructorArgs" /> will be used for
        ///     all mocked types.
        /// </summary>
        /// <param name="typesToMock"></param>
        /// <param name="mockConstructorArgs"></param>
        public CreateFromMockAttribute(Type[] typesToMock, params object[] mockConstructorArgs)
        {
            this.typesToMock = typesToMock;
            this.mockConstructorArgs = mockConstructorArgs;
        }

        /// <remarks>
        ///     <list type="bullet">
        ///         <item>The default mocking library is Moq, used via <see cref="MoqMocker" />. </item>
        ///         <item>
        ///             <strong>Note that your test project still needs a project reference to Moq or your chosen mocking library</strong>
        ///             in order to create mocks.
        ///         </item>
        ///     </list>
        /// </remarks>
        public IMockingLibraryAdapter MockingLibraryAdapter { get; set; }
        
        /// <inheritdoc />
        public object CreateInstance(Type type, IEnumerable<Type> typesWaitingToBeBuilt, object searchAnchor)
        {
            if (!typesToMock.Contains(type)) return null;
            //
            EnsureMockingLibraryAdapter();
            MockingLibraryAdapter.EnsureMockingAssemblyIsLoadedAndWorkingElseThrow();
            return MockingLibraryAdapter.CreateMockElseNull(type, mockConstructorArgs);
        }

        public static bool IsAKnownMock(object value)
        {
            return KnownMockingLibraryAdapters
                .OfType<IMockingLibraryAdapterWithInspections>()
                .Any(m => m.IsThisMyMockObject(value));
        }

        void EnsureMockingLibraryAdapter() { MockingLibraryAdapter = MockingLibraryAdapter ?? MoqMocker.Instance; }
    }
}