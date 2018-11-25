using System;
using System.Collections.Generic;
using System.Reflection;

namespace ActivateAnything
{
    /// <summary>
    ///     A marker interface for any kind of ActivateAnything rule understood by <see cref="AnythingActivator" />
    /// </summary>
    public interface IActivateAnythingRule
    {
    }

    /// <summary>An ActivateAnything rule which will create an instance of a concrete <c>Type</c>.</summary>
    public interface IActivateInstanceRule : IActivateAnythingRule
    {
        /// <summary>Create an instance of <paramref name="type"/> if possible, null if not</summary>
        /// <param name="type"></param>
        /// <param name="typesWaitingToBeBuilt">optional additional context: the stack of types awaiting construction which
        /// has led to a request for this <paramref name="type"/></param>
        /// <param name="searchAnchor">optional additional context: the <see cref="AnythingActivator.SearchAnchor"/> of the
        /// current search</param>
        /// <returns>An instance of <paramref name="type"/> if possible, null if not</returns>
        object CreateInstance(Type type, IEnumerable<Type> typesWaitingToBeBuilt, object searchAnchor = null);
    }

    /// <summary>An ActivateAnything rule which will choose between constructors of a concrete <c>Type</c> to be
    /// instantiated.</summary>
    public interface IChooseConstructorRule : IActivateAnythingRule
    {
        /// <summary>Chooses one of <see cref="Type.GetConstructors()"/> for <paramref name="type"/> which satisfies this
        /// <see cref="IChooseConstructorRule"/>'s criteria. Null if none of them do so.</summary>
        /// <param name="type"></param>
        /// <param name="typesWaitingToBeBuilt">optional additional context: the stack of types awaiting construction which
        /// has led to a request for this <paramref name="type"/></param>
        /// <param name="searchAnchor">optional additional context: the <see cref="AnythingActivator.SearchAnchor"/> of the
        /// current search</param>
        /// <returns>One of <see cref="Type.GetConstructors()"/> for <paramref name="type"/> which satisfies this
        /// <see cref="IChooseConstructorRule"/>'s criteria. Null if none of them do so.</returns>
        ConstructorInfo ChooseConstructor(Type type, IEnumerable<Type> typesWaitingToBeBuilt, object searchAnchor = null);
    }

    /// <summary>
    ///     An ActivateAnything rule which searches for (or dynamically creates)
    ///     a concrete <c>Type</c> assignable to a requested <c>Type</c>.
    /// </summary>
    public interface IFindTypeRule : IActivateAnythingRule
    {
        /// <summary>Attempts to find a concrete <see cref="Type"/> – one that is not abstract, not an interface, not a
        /// generic Type definition –  which is assignable to <paramref name="type"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typesWaitingToBeBuilt"></param>
        /// <param name="searchAnchor"></param>
        /// <returns>If possible, a concrete <see cref="Type"/> – one that is not abstract, not an interface, not a generic Type
        /// definition – which is assignable to <paramref name="type"/>. Otherwise, returns null.</returns>
        Type FindTypeAssignableTo(Type type, IEnumerable<Type> typesWaitingToBeBuilt = null, object searchAnchor = null);
    }

    /// <summary>
    ///     An ActivateAnything rule which searches for (or dynamically creates)
    ///     a concrete <c>Type</c> whose name ends with a given string.
    /// </summary>
    public interface IFindTypeByNameRule : IActivateAnythingRule
    {
        /// <summary>Attempts to find a concrete <see cref="Type"/> – one that is not abstract, not an interface, not a
        /// generic Type definition –  whose <see cref="MemberInfo.Name"/> ends with <paramref name="typeNameEndingWith"/>
        /// </summary>
        /// <param name="typeNameEndingWith"></param>
        /// <param name="typesWaitingToBeBuilt"></param>
        /// <param name="searchAnchor"></param>
        /// <returns>If possible, a concrete <see cref="Type"/> – one that is not abstract, not an interface, not a
        /// generic Type definition –  whose <see cref="MemberInfo.Name"/> ends with <paramref name="typeNameEndingWith"/>.
        /// Otherwise, returns null.</returns>
        Type FindTypeAssignableTo(
            string            typeNameEndingWith,
            IEnumerable<Type> typesWaitingToBeBuilt = null,
            object            searchAnchor          = null);
    }
}
