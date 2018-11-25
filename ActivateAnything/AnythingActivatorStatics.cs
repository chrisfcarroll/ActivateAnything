using System.Collections.Generic;
using System.Linq;

namespace ActivateAnything
{
    public partial class AnythingActivator
    {
        /// <summary>
        ///     An instance of <see cref="AnythingActivator" /> which uses <see cref="DefaultRules" />
        ///     and has no <see cref="SearchAnchor" />.
        /// </summary>
        public static AnythingActivator Instance = FromDefaultRules();

        /// <summary>Identical to <see cref="ActivateDefaultRules.AllDefaultRules" /> </summary>
        public static IReadOnlyCollection<IActivateAnythingRule> DefaultRules => ActivateDefaultRules.AllDefaultRules;

        /// <summary>
        ///     Returns a new instance of <see cref="AnythingActivator" /> which uses <see cref="DefaultRules" />
        ///     and has no <see cref="SearchAnchor" />.
        /// </summary>
        public static AnythingActivator FromDefaultRules() { return new AnythingActivator(DefaultRules); }

        /// <summary>
        ///     An instance of <see cref="AnythingActivator" /> which uses <see cref="DefaultRules" /> for its
        ///     <see cref="Rules" /> and uses <paramref name="searchAnchor" /> for its <see cref="SearchAnchor" />
        /// </summary>
        /// <param name="searchAnchor">The <see cref="SearchAnchor" /> to use.</param>
        public static AnythingActivator FromAnchor(object searchAnchor)
        {
            return new AnythingActivator(searchAnchor, DefaultRules);
        }

        /// <summary>
        ///     Returns a new instance of <see cref="AnythingActivator" /> which uses <paramref name="moreRules" /> then
        ///     <see cref="DefaultRules" />, and has no <see cref="SearchAnchor" />.
        /// </summary>
        public static AnythingActivator FromDefaultRulesAnd(params IActivateAnythingRule[] moreRules)
        {
            return new AnythingActivator(moreRules.Union(DefaultRules));
        }

        /// <summary>
        ///     An instance of <see cref="AnythingActivator" /> which uses <see cref="DefaultRules" />
        ///     and uses <paramref name="searchAnchor" /> as its <see cref="SearchAnchor" />
        /// </summary>
        public static AnythingActivator FromDefaultRules(object searchAnchor)
        {
            return new AnythingActivator(searchAnchor, DefaultRules);
        }

        /// <summary>
        ///     An instance of <see cref="AnythingActivator" /> which uses <paramref name="moreRules" /> and
        ///     <see cref="DefaultRules" />, and uses <paramref name="searchAnchor" /> as its <see cref="SearchAnchor" />
        /// </summary>
        public static AnythingActivator FromDefaultRulesAnd(object searchAnchor, params IActivateAnythingRule[] moreRules)
        {
            return new AnythingActivator(searchAnchor, moreRules.Union(DefaultRules));
        }

        /// <summary>
        ///     An instance of <see cref="AnythingActivator" /> which uses <paramref name="moreRules" /> and then
        ///     <see cref="DefaultRules" />, and uses <paramref name="searchAnchor" /> as its <see cref="SearchAnchor" />
        /// </summary>
        public static AnythingActivator FromDefaultRulesAnd(object searchAnchor, IEnumerable<IActivateAnythingRule> moreRules)
        {
            return new AnythingActivator(searchAnchor, moreRules.Union(DefaultRules));
        }

        /// <summary>
        ///     Create an <see cref="AnythingActivator" /> using <paramref name="searchAnchorAndRuleProvider" /> as
        ///     the <see cref="SearchAnchor" /> and also as the source <see cref="IActivateAnythingRule" />s.
        ///     Rules are obtained from the <see cref="Type.CustomAttributes" /> of <paramref name="searchAnchorAndRuleProvider" />
        ///     .
        ///     <seealso cref="TypeExtensions.GetActivateAnythingRuleAttributes" />
        /// </summary>
        /// <param name="searchAnchorAndRuleProvider">
        ///     will be used as the <see cref="SearchAnchor" /> and as the sources of
        ///     <see cref="Rules" />
        /// </param>
        public static AnythingActivator FromDefaultAndSearchAnchorRules(object searchAnchorAndRuleProvider)
        {
            return FromDefaultRulesAnd(
            searchAnchorAndRuleProvider,
            searchAnchorAndRuleProvider.GetType().GetActivateAnythingRuleAttributes());
        }

        /// <summary>
        ///     Create an <see cref="AnythingActivator" /> using <paramref name="moreRules" />, then using rules on
        ///     <paramref name="searchAnchorAndRuleProvider" />, then using <see cref="DefaultRules" />; and using
        /// </summary>
        /// <param name="searchAnchorAndRuleProvider">
        ///     will be used as the <see cref="SearchAnchor" /> and as the sources of
        ///     <see cref="Rules" />
        /// </param>
        /// <param name="moreRules"></param>
        public static AnythingActivator FromDefaultAndSearchAnchorRulesAnd(
        object searchAnchorAndRuleProvider,
        params IActivateAnythingRule[] moreRules)
        {
            return new AnythingActivator(searchAnchorAndRuleProvider,
            moreRules
            .Union(searchAnchorAndRuleProvider.GetType().GetActivateAnythingRuleAttributes())
            .Union(DefaultRules));
        }

        /// <summary>
        ///     Create an <see cref="AnythingActivator" /> using <paramref name="moreRules" />, then using rules on
        ///     <paramref name="searchAnchorAndRuleProvider" />, then using <see cref="DefaultRules" />; and using
        /// </summary>
        /// <param name="searchAnchorAndRuleProvider">
        ///     will be used as the <see cref="SearchAnchor" /> and as the sources of
        ///     <see cref="Rules" />
        /// </param>
        /// <param name="moreRules"></param>
        public static AnythingActivator FromDefaultAndSearchAnchorRulesAnd(
        object searchAnchorAndRuleProvider,
        IEnumerable<IActivateAnythingRule> moreRules)
        {
            return new AnythingActivator(searchAnchorAndRuleProvider,
            moreRules
            .Union(searchAnchorAndRuleProvider.GetType().GetActivateAnythingRuleAttributes())
            .Union(DefaultRules));
        }
    }
}
