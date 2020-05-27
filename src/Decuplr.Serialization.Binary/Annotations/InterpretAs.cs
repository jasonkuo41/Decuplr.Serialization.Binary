using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary.Annotations {

    // TODO : Maybe hold on to this for future ?
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class FormatAsAttribute : Attribute {
        // Check if type is also BinaryFormat, or BinarySealedFormat
        public FormatAsAttribute(Type type) {
            Type = type;
        }

        public Type Type { get; }
    }
}
