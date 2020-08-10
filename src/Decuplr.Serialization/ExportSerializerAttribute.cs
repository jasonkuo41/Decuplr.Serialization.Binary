using System;

namespace Decuplr.Serialization {

    /// <summary>
    /// Exports the serializer as a public API for consumption
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public sealed class ExportSerializerAttribute : Attribute {
        /// <summary>
        /// Exports the serializer with the corresponding type to a public API.
        /// </summary>
        /// <param name="kind">The serializing kind</param>
        /// <param name="parsingType">The type serializer is capable of serializing</param>
        /// <param name="exportSerialierName">The exporting serializing class name</param>
        public ExportSerializerAttribute(SerializerKind kind, Type parsingType, string exportSerialierName) {
            ExportSerializerName = exportSerialierName;
            ParsingType = parsingType;
            SerializerKind = kind;
        }

        /// <summary>
        /// The type serializer is capable of serializing
        /// </summary>
        public Type ParsingType { get; }

        /// <summary>
        /// The name of the serializer
        /// </summary>
        public string ExportSerializerName { get; }

        /// <summary>
        /// The serializer kind
        /// </summary>
        public SerializerKind SerializerKind { get; }

        /// <summary>
        /// The namespace where the serializer will be generated.
        /// </summary>
        public string? ExportNamespace { get; set; }
    }
}
