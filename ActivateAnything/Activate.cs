using System;
using System.Reflection;

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
    ///     type. For instance, the <see cref="CreateFromFactoryMethod" /> rule.
    /// </item>
    /// <item>
    ///     <see cref="IFindTypeRule" /> provides rules for where to look for candidate
    ///     concrete subTypes of an abstract type.
    /// </item>
    /// <item>
    ///     <see cref="IChooseConstructorRule" /> rules for how to choose between constructors when
    ///     the <see cref="IFindTypeRule" />s have found a concrete <c>Type</c> to instantiate.
    /// </item>
    /// </list>
    ///
    /// <seealso cref="AnythingActivator"/>
    /// <seealso cref="IActivateAnythingRule"/>
    /// <seealso cref="IFindTypeRule"/>
    /// <seealso cref="IChooseConstructorRule"/>
    /// <seealso cref="IActivateInstanceRule"/>
    /// 
    /// </summary>
    public static class Activate
    {
        /// <summary>
        ///     Creates an instance of something assignable to <typeparamref name="T" /> using <see cref="AnythingActivator.DefaultRules" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>An instance of type <typeparamref name="T" /> if possible, <c>default(T)</c> if unable to construct one</returns>        
        public static T New<T>() => AnythingActivator.Instance.New<T>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchAnchor">
        ///     An object used by some <see cref="AnythingActivator.Rules" />, especially, <see cref="IFindTypeRule" />
        ///     rules, as a reference point—whether as a starting point or as a limit—to their search. For instance, the
        ///     <see cref="FindInAnchorAssembly" /> rule will only look for concrete types in
        ///     <c>SearchAnchor.GetType().Assembly</c>, and <see cref="FindInAssembliesReferencedByAnchorAssembly" /> rule will
        ///     look in each <see cref="Assembly" /> referenced by <c>SearchAnchor.GetType().Assembly</c>.
        /// </param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T New<T>(object searchAnchor)
        {
            return AnythingActivator.FromDefaultRules(searchAnchor).New<T>();
        }

        /// <summary>
        ///     Creates an instance of something assignable to <typeparamref name="T" /> using rules found
        ///     on <paramref name="searchAnchor"/>, then <paramref name="moreRules"/>, then
        ///     <see cref="AnythingActivator.DefaultRules" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchAnchor">
        ///     An object used by some <see cref="AnythingActivator.Rules" />, especially, <see cref="IFindTypeRule" />
        ///     rules, as a reference point—whether as a starting point or as a limit—to their search. For instance, the
        ///     <see cref="FindInAnchorAssembly" /> rule will only look for concrete types in
        ///     <c>SearchAnchor.GetType().Assembly</c>, and <see cref="FindInAssembliesReferencedByAnchorAssembly" /> rule will
        ///     look in each <see cref="Assembly" /> referenced by <c>SearchAnchor.GetType().Assembly</c>.
        /// </param>
        /// <param name="moreRules">Rules to override the default <see cref="AnythingActivator.Rules"/></param>
        /// <returns>An instance of type <typeparamref name="T" /> if possible, <c>default(T)</c> if unable to construct one</returns>        
        public static T FromDefaultRulesAnd<T>(object searchAnchor, params IActivateAnythingRule[] moreRules)
        {
            return AnythingActivator.FromDefaultAndSearchAnchorRulesAnd(searchAnchor, moreRules).New<T>();
        }

        /// <summary>
        ///     Creates an instance of something assignable to <typeparamref name="T" /> using
        ///     <paramref name="moreRules"/> then <see cref="AnythingActivator.DefaultRules" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="moreRules">Rules to override the default <see cref="AnythingActivator.Rules"/></param>
        /// <returns>An instance of type <typeparamref name="T" /> if possible, <c>default(T)</c> if unable to construct one</returns>        
        public static T FromDefaultRulesAnd<T>(params IActivateAnythingRule[] moreRules)
        {
            return AnythingActivator.FromDefaultRulesAnd(moreRules).New<T>();
        }
    }
}