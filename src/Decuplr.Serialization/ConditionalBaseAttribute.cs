using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization {

    /// <summary>
    /// A kind of attribute that allows context to be executed under certain conditions
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class ConditionalBaseAttribute : Attribute {
        
        /// <param name="conditionalMember">The property, field or method to be evaulated, must return <see cref="bool"/></param>
        protected ConditionalBaseAttribute(string conditionalMember)
            : this(conditionalMember, Condition.Equal, true) {
        }

        /// <param name="targetMember">The property, field or method to be evaulated</param>
        /// <param name="value">The value to be evaulated with</param>
        protected ConditionalBaseAttribute(string targetMember, object value)
            : this(targetMember, Condition.Equal, value) {
        }

        /// <param name="member">The property, field or method to be evaulated</param>
        /// <param name="condition">The operand to decide the comparsion type</param>
        /// <param name="value">The value to be evaulated with</param>
        protected ConditionalBaseAttribute(string member, Condition condition, object value) {
            Expression = new ConditionExpression(member) {
                Condition = condition,
                ComparedValue = value
            };
        }

        public ConditionExpression Expression { get; }
    }
}
