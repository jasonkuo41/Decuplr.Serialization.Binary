using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.Serialization.Binary.Annotations.Namespaces;

namespace Decuplr.Serialization.Binary {

    /// <summary>
    /// Marks the underlying data should be serialized with the given endianess
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    [ApplyNamespaceIf("Internal.BigEndianess", ByteOrder.BigEndian)]
    [ApplyNamespaceIf("Default", ByteOrder.LittleEndian)]
    [DefinesPrimitives]
    public sealed class EndianessAttribute : Attribute, INamespacePrioritizable {
        public EndianessAttribute(ByteOrder endianess) {
            Endianess = endianess;
        }

        public ByteOrder Endianess { get; }
        public int PrioritizeIndex { get; set; }
    }
}
