using System;
using System.Collections.Generic;
using System.Reflection;

namespace ActivateAnything
{
    /// <summary>
    /// Find a Type by just providing one. This rule doesn't search anywhere, you simply
    /// tell it the <see cref="TypeToUse"/> when a <see cref="RequiredType"/> is asked for.
    /// Note that this rule will not match Ancestor Types of <see cref="RequiredType"/>, it
    /// will only match <see cref="RequiredType"/> or <see cref="RequiredType"/>.<see cref="Type.Name"/>
    /// </summary>
    public class ChooseExactType : FindTypeRule
    {
        /// <summary>
        /// The Type to match. Note that this rule will only be triggered if the type or type name passed to
        /// <see cref="FindTypeAssignableTo(string,System.Collections.Generic.IEnumerable{System.Type},object)"/>
        /// or  <see cref="FindTypeAssignableTo(System.Type,System.Collections.Generic.IEnumerable{System.Type},object)"/>
        /// is the same is this type. Ancestor types will not match.
        /// </summary>
        public Type RequiredType { get; }
        
        /// <summary>The Type to return if <see cref="RequiredType"/> is a match for the find request.</summary>
        public Type TypeToUse { get; }

        /// <summary>Always return <paramref name="typeToUse"/> when <paramref name="requiredType"/> is searched for.</summary>
        /// <param name="requiredType">When this type is searched for, return <paramref name="typeToUse"/></param>
        /// <param name="typeToUse">The type to instantiate when <paramref name="requiredType"/> is searched for</param>
        public ChooseExactType(Type requiredType, Type typeToUse)
        {
            this.RequiredType = requiredType;
            this.TypeToUse    = typeToUse;
        }

        /// <summary>Always return <param name="requiredType"></param> when it is searched for.</summary>
        /// <param name="requiredType">The Type to return when it is searched for.</param>
        public ChooseExactType(Type requiredType)
        {
            this.RequiredType = requiredType;
            this.TypeToUse    = requiredType;
        }
        /// <summary></summary>
        /// <param name="typeName"></param>
        /// <param name="typesWaitingToBeBuilt"></param>
        /// <param name="searchAnchor"></param>
        /// <returns>If <c><paramref name="typeName"/> == <see cref="RequiredType"/>.<see cref="MemberInfo.Name"/></c>
        /// then return <see cref="TypeToUse"/>.
        /// Otherwise, returns null.</returns>
        public override Type FindTypeAssignableTo(
            string            typeName,
            IEnumerable<Type> typesWaitingToBeBuilt = null,
            object            searchAnchor          = null)
        {
            return RequiredType.Name ==typeName ? TypeToUse : null;
        }

        /// <summary></summary>
        /// <param name="type"></param>
        /// <param name="typesWaitingToBeBuilt"></param>
        /// <param name="searchAnchor"></param>
        /// <returns>If <c><paramref name="type"/> == <see cref="RequiredType"/></c> then
        /// return <see cref="TypeToUse"/>.
        /// Otherwise, return null.</returns>
        public override Type FindTypeAssignableTo(
            Type              type,
            IEnumerable<Type> typesWaitingToBeBuilt = null,
            object            searchAnchor          = null)
        {
            return RequiredType ==type ? TypeToUse : null;
        }
    }
}
