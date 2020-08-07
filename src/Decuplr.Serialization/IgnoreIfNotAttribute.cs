using System;

namespace Decuplr.Serialization {

    /// <summary>
    /// Annotates the member to be ignored if the conditions are not met, the opposite of <see cref="IgnoreIfAttribute"/>.
    /// </summary>
    public sealed class IgnoreIfNotAttribute : ConditionalBaseAttribute {

        /// <summary>
        /// Ignores the member if <paramref name="conditionalMember"/> is evaluated to false. Can only target functions that return <see cref="bool"/>
        /// </summary>
        /// <inheritdoc/>
        public IgnoreIfNotAttribute(string conditionalMember)
            : base(conditionalMember) {
        }

        /// <summary>
        /// Ignores the member if <paramref name="targetMember"/> is not equal to <paramref name="value"/>
        /// </summary>
        /// <inheritdoc/>
        public IgnoreIfNotAttribute(string targetMember, object value)
            : base(targetMember, value) {
        }

        /// <summary>
        /// Ignores the member if <paramref name="member"/> doesn't match the condition (via <paramref name="operator"/>) compared to <paramref name="value"/>
        /// </summary>
        /// <inheritdoc/>
        public IgnoreIfNotAttribute(string member, Condition @operator, object value)
            : base(member, @operator, value) {
        }

    }



}
