using System;

// Move to Decuplr.Serialization.Annotations
namespace Decuplr.Serialization {

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class IgnoreIfAttribute : ConditionalBaseAttribute {
        /// <summary>
        /// Ignores the member if <paramref name="conditionalMember"/> is evaluated to true. Can only target functions that return <see cref="bool"/>
        /// </summary>
        /// <param name="conditionalMember">The property, field or method to be evaulated, must return <see cref="bool"/></param>
        public IgnoreIfAttribute(string conditionalMember)
            : base(conditionalMember) {
        }

        /// <summary>
        /// Ignores the member if <paramref name="targetMember"/> is equal to <paramref name="value"/>
        /// </summary>
        /// <param name=" targetMember">The property, field or method to be evaulated</param>
        /// <param name="value">The value to be evaulated with</param>
        public IgnoreIfAttribute(string targetMember, object value)
            : base(targetMember, value) {
        }

        /// <summary>
        /// Ignores the member if <paramref name="member"/> matches the condition (via <paramref name="operand"/>) compared to <paramref name="value"/>
        /// </summary>
        /// <inheritdoc/>
        public IgnoreIfAttribute(string member, Condition operand, object value)
            : base(member, operand, value) {
        }

    }



}
