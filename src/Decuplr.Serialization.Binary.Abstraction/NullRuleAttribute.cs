using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary {

    /// <summary>
    /// By default the binary serializer cannot deal with null value,
    /// this attribute is to state the action that should be taken when encountering one other then the default action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class NullRuleAttribute : Attribute {

        public NullRuleAttribute(NullAction action) {
            Action = action;
        }

        /// <summary>
        /// Format as a kind when it's null
        /// </summary>
        /// <param name="type">The type to be formatted as</param>
        public NullRuleAttribute(Type type) {
            FormatAs = type;
        }

        /// <summary>
        /// The action to take when the underlying value is null, null if <see cref="FormatAs"/> is not null
        /// </summary>
        public NullAction? Action { get; }

        /// <summary>
        /// The type to format as when the type is null, null if <see cref="Action"/> is not null
        /// </summary>
        public Type FormatAs { get; }
    }
}
