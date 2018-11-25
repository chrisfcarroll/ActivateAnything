namespace ActivateAnything
{
    /// <summary>
    ///     By default, these Assemblies will be ignored when a <see cref="IFindTypeRule" />
    ///     searches for a Type.
    /// </summary>
    public static class DefaultAssembliesToIgnore
    {
        /// <summary>
        ///     Default value: <c>{ "mscorlib", "System", "nunit","Microsoft.VisualStudio", "Moq", "Xunit" }</c>
        /// </summary>
        public static readonly string[] ByName = {"mscorlib", "System", "nunit", "Microsoft.VisualStudio", "Moq", "Xunit"};
    }
}
