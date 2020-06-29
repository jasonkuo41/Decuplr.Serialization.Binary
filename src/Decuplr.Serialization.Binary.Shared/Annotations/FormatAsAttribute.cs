using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary.Annotations {

    // TODO : Maybe hold on to this for future ?
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class FormatAsAttribute : Attribute {
        // Check if type is also BinaryFormat, or BinarySealedFormat
        /// <summary>
        /// Format 
        /// </summary>
        /// <param name="type"></param>
        public FormatAsAttribute(Type type) {
            Type = type;
        }

        public FormatAsAttribute(Type type, string valueSource) {
            Type = type;
            ValueSource = valueSource;
        }

        public FormatAsAttribute(Type type, string valueSource, Operator operand, object value) {

        }

        public Type Type { get; }
        public string ValueSource { get; }
    }
}
