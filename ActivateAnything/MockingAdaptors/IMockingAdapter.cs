using System;
using System.IO;

namespace ActivateAnything
{
    /// <summary>
    ///     Used by <see cref="CreateFromMock" /> rule to create mocks using the specified mocking library.
    ///     Moq is the default mocking library, and <see cref="MoqMocker" /> is the default adapter.
    ///     Note however that TestBase does not have a dependency on Moq;
    ///     in order to use Moq or a different mocking library in your tests, you should include a reference to it in your test
    ///     project.
    /// </summary>
    /// <remarks>Implement this interface to use your preferred Mocking Library instead of Moq</remarks>
    public interface IMockingAdapter
    {
        /// <summary>
        ///     Try to create a Mock for an object of type <paramref name="type" />.
        ///     On failure, return Null.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="mockConstructorArgs"></param>
        /// <returns></returns>
        object CreateMockElseNull(Type type, params object[] mockConstructorArgs);

        /// <summary>
        ///     Try to create a Mock for an object of type <paramref name="type" />.
        ///     On failure, throw an Exception.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="mockConstructorArgs"></param>
        /// <returns></returns>
        object CreateMockElseThrow(Type type, params object[] mockConstructorArgs);

        /// <returns>
        ///     <c>false</c> if the adapter is unable to find the expected Mocking library dll;
        ///     <c>true</c> if it is.
        /// </returns>
        /// <remarks>
        ///     A Mocking library adapter should be able to load its Assembly (i.e. the dll or exe) from the "usual place",
        ///     i.e. the <see cref="AppDomain.CurrentDomain.BaseDirectory" />.
        /// </remarks>
        bool IsMockingAssemblyFound();

        /// <summary>
        ///     This method will be called whenever <see cref="CreateFromMock.MockingAdapter" /> is set, to ensure
        ///     that the required mocking library is working.
        /// </summary>
        /// <exception cref="FileNotFoundException">
        ///     thrown if the mocking library Assembly file (i.e. the dll or exe) cannot be
        ///     found
        /// </exception>
        /// <remarks>
        ///     A mocking library adapter should throw a helpful-to-a-developer Exception message if it cannot create a mock.
        ///     In particular, it should throw a <see cref="FileNotFoundException" /> if that is the problem.
        /// </remarks>
        void EnsureMockingAssemblyIsLoadedAndWorkingElseThrow();
    }
}
