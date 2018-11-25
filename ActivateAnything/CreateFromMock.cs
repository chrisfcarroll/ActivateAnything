using System;
using System.Collections.Generic;
using System.Linq;

namespace ActivateAnything
{
    /// <inheritdoc cref="IActivateAnythingRule" />
    public class CreateFromMock : Attribute, IActivateInstanceRule
    {
        /// <summary>The list of Mocking adapters known to this version of ActivateAything.
        /// This version contains <c>{ <see cref="MoqMocker.Instance"/> }</c>
        /// </summary>
        public static List<IMockingAdapter> KnownMockingLibraryAdapters 
                = new List<IMockingAdapter> {MoqMocker.Instance};

        readonly object[] mockConstructorArgs;

        readonly Type[] typesToMock;

        /// <inheritdoc />
        public CreateFromMock(Type typeToMock, params object[] mockConstructorArgs)
        {
            this.mockConstructorArgs = mockConstructorArgs;
            typesToMock = new[] {typeToMock};
        }

        /// <inheritdoc />
        public CreateFromMock(params Type[] typesToMock)
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
        public CreateFromMock(Type[] typesToMock, params object[] mockConstructorArgs)
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
        public IMockingAdapter MockingAdapter { get; set; }
        
        /// <inheritdoc />
        public object CreateInstance(Type type, IEnumerable<Type> typesWaitingToBeBuilt, object searchAnchor)
        {
            if (!typesToMock.Contains(type)) return null;
            //
            EnsureMockingLibraryAdapter();
            MockingAdapter.EnsureMockingAssemblyIsLoadedAndWorkingElseThrow();
            return MockingAdapter.CreateMockElseNull(type, mockConstructorArgs);
        }

        /// <summary>Test whether any <see cref="KnownMockingLibraryAdapters"/> recognises
        /// this object as a Mock object they created, using 
        /// <see cref="IMockingAdapterInspections.IsThisMyMockObject"/>
        /// </summary>
        /// <param name="value"></param>
        /// <seealso cref="IMockingAdapterInspections.IsThisMyMockObject"/>
        /// <returns></returns>
        public static bool IsAKnownMock(object value)
        {
            return KnownMockingLibraryAdapters
                .OfType<IMockingAdapterInspections>()
                .Any(m => m.IsThisMyMockObject(value));
        }

        void EnsureMockingLibraryAdapter() { MockingAdapter = MockingAdapter ?? MoqMocker.Instance; }
    }
}