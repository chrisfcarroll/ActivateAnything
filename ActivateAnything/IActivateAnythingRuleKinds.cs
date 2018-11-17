using System;
using System.Collections.Generic;
using System.Reflection;

namespace ActivateAnything
{
    /// <summary>A marker interface for any kind of ActivateAnything rule understood by <see cref="AnythingActivator" /></summary>
    public interface IActivateAnythingRule{}

    /// <summary>An ActivateAnything rule which will create an instance of a concrete <c>Type</c>.</summary>
    public interface IActivateInstanceRule : IActivateAnythingRule
    {
        object CreateInstance(Type type, IEnumerable<Type> typesWaitingToBeBuilt, object searchAnchor);
    }

    /// <summary>An ActivateAnything rule which will choose between constructors of a concrete <c>Type</c> to be instantiated.</summary>
    public interface IActivateAnythingChooseConstructorRule : IActivateAnythingRule
    {
        ConstructorInfo ChooseConstructor(Type type, IEnumerable<Type> typesWaitingToBeBuilt, object searchAnchor = null);
    }

    /// <summary>
    ///     An ActivateAnything rule which searches for (or dynamically creates)
    ///     a concrete <c>Type</c> assignable to a requested <c>Type</c>.
    /// </summary>
    public interface IActivateConcreteTypeRule : IActivateAnythingRule
    {
        Type FindTypeAssignableTo(Type type, IEnumerable<Type> typesWaitingToBeBuilt = null, object getType = null);
    }

    /// <summary>
    ///     An ActivateAnything rule which searches for (or dynamically creates)
    ///     a concrete <c>Type</c> whose name ends with a given string.
    /// </summary>
    public interface IActivateConcreteTypeByNameRule : IActivateAnythingRule
    {
        Type FindTypeAssignableTo(
            string typeNameEndingWith,
            IEnumerable<Type> typesWaitingToBeBuilt = null,
            object getType = null);
    }
}