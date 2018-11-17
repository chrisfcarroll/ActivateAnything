using System.Linq;

namespace ActivateAnything
{
    /// <summary>
    /// Factory methods which use an <see cref="AnythingActivator"/> to create instances of
    /// a <see cref="Type" />, whether or not the Type is concrete, whether or not the Type has constructor
    /// dependencies, whether or not any suitable concrete Subtype is currently loaded or known.
    /// The attempts to find and instantiate suitable Types is driven by <see cref="AnythingActivator.Rules" /> and
    /// <see cref="AnythingActivator.SearchAnchor" />.
    /// There are three kinds of <see cref="IActivateAnythingRule" /> Rule.
    /// <list type="bullet">
    /// <item>
    ///     <see cref="IActivateInstanceRule" /> provides an immediate source of a concrete
    ///     type. For instance, the <see cref="CreateFromFactoryMethodAttribute" /> rule.
    /// </item>
    /// <item>
    ///     <see cref="IActivateConcreteTypeRule" /> provides rules for where to look for candidate
    ///     concrete subTypes of an abstract type.
    /// </item>
    /// <item>
    ///     <see cref="IActivateAnythingChooseConstructorRule" /> rules for how to choose between constructors when
    ///     the <see cref="IActivateConcreteTypeRule" />s have found a concrete <c>Type</c> to instantiate.
    /// </item>
    /// </list>
    ///
    /// <seealso cref="AnythingActivator"/>
    /// <seealso cref="IActivateAnythingRule"/>
    /// <seealso cref="IActivateConcreteTypeRule"/>
    /// <seealso cref="IActivateAnythingChooseConstructorRule"/>
    /// <seealso cref="IActivateInstanceRule"/>
    /// 
    /// </summary>
    public static class Activate
    {
        public static T FromDefaultRules<T>() => AnythingActivator.Instance.New<T>();

        public static T FromDefaultRules<T>(object searchAnchor)
        {
            return AnythingActivator.FromDefaultRules(searchAnchor).New<T>();
        }

        public static T FromDefaultRulesAnd<T>(object searchAnchor, params IActivateAnythingRule[] moreRules)
        {
            return AnythingActivator.FromDefaultRulesAnd(searchAnchor, moreRules).New<T>();
        }

        public static T FromDefaultRulesAnd<T>(params IActivateAnythingRule[] moreRules)
        {
            return AnythingActivator.FromDefaultRulesAnd(moreRules).New<T>();
        }
    }
}