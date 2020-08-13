using System;
using Decuplr.Serialization.Annotations;

// Move to Decuplr.Serialization.Annotations
namespace Decuplr.Serialization.Binary {

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class IgnoreIfAttribute : Attribute {
        /// <summary>
        /// Ignores the member if <paramref name="conditionalMember"/> is evaluated to true. Can only target functions that return <see cref="bool"/>
        /// </summary>
        /// <param name="conditionalMember">The property, field or method to be evaulated, must return <see cref="bool"/></param>
        public IgnoreIfAttribute(string conditionalMember)
            : this(conditionalMember, Operator.Equal, true) {
        }

        /// <summary>
        /// Ignores the member if <paramref name="targetMember"/> is equal to <paramref name="value"/>
        /// </summary>
        /// <param name=" targetMember">The property, field or method to be evaulated</param>
        /// <param name="value">The value to be evaulated with</param>
        public IgnoreIfAttribute(string targetMember, object value)
            : this(targetMember, Operator.Equal, value) {
        }

        /// <summary>
        /// Ignores the member if <paramref name="member"/> matches the condition (via <paramref name="operand"/>) compared to <paramref name="value"/>
        /// </summary>
        /// <param name="member">The property, field or method to be evaulated</param>
        /// <param name="operand">The operand to decide the comparsion type</param>
        /// <param name="value">The value to be evaulated with</param>
        public IgnoreIfAttribute(string member, Operator operand, object value) {
            ComparedMember = member;
            Operator = operand;
            ComparedValue = value;
        }

        public string ComparedMember { get; }
        public Operator Operator { get; }
        public object ComparedValue { get; }
    }



}
