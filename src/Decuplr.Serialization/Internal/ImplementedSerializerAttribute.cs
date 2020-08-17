using System;
using System.ComponentModel;

namespace Decuplr.Serialization.Internal {

    //[EditorBrowsable(EditorBrowsableState.Never)]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ImplementedSerializerAttribute : Attribute {

        public ImplementedSerializerAttribute(SerializerKind kind, Type serializedType, params string[] exportedNamespaces) {
            Kind = kind;
            SerializedType = serializedType;
            ExportedNamespaces = exportedNamespaces;
        }

        public SerializerKind Kind { get; }

        public Type SerializedType { get; }

        public string[] ExportedNamespaces { get; }

        public Type? PublicExportType { get; set; }

    }
}
