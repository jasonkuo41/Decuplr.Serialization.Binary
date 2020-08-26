using System;

namespace Decuplr.Serialization {

    // Note: We should check the user's whole project and make sure there isn't a duplicate export serializer.

    /// <summary>
    /// Exports the serializer as a public API for consumption
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public sealed class ExportSerializerAttribute : Attribute {

        /// <summary>
        /// Exports the serializer to the corresponding type for pulic consumption with explicit arguments.
        /// </summary>
        /// <param name="kind">The serializing kind</param>
        /// <param name="parsingType">The type serializer is capable of serializing</param>
        /// <param name="serializerType">The exporting serializer</param>
        public ExportSerializerAttribute(SerializerKind kind, Type parsingType, Type serializerType) {
            SerializerKind = kind;
            ParsingType = parsingType;
            ExportSerializer = serializerType;
        }

        /// <summary>
        /// Implicitly export the serializer to the corresponding type for pulic consumption with the <see cref="Serialization.SerializerKind"/> explicitly stated.
        /// </summary>
        /// <inheritdoc cref="ExportSerializerAttribute(SerializerKind, Type, Type)"/>
        public ExportSerializerAttribute(SerializerKind kind, Type serializerType) {
            SerializerKind = kind;
            ExportSerializer = serializerType;
        }

        /// <summary>
        /// Implicitly export the serializer to the corresponding type for pulic consumption.
        /// </summary>
        /// <inheritdoc cref="ExportSerializerAttribute(SerializerKind, Type, Type)"/>
        public ExportSerializerAttribute(Type serializerType) {
            ExportSerializer = serializerType;
        }

        /// <summary>
        /// The type serializer is capable of serializing, null when the parsing type is determinated implicitly.
        /// </summary>
        public Type? ParsingType { get; }

        /// <summary>
        /// The exported serializer
        /// </summary>
        public Type ExportSerializer { get; }

        /// <summary>
        /// The serializer kind, null if the serializer kind is determinated implicitly.
        /// </summary>
        public SerializerKind? SerializerKind { get; }

    }
}
