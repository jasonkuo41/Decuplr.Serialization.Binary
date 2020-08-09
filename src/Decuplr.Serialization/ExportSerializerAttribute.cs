using System;

namespace Decuplr.Serialization {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public sealed class ExportSerializerAttribute : Attribute {
        public ExportSerializerAttribute(string exportSerializer, Type parsingType) {
            ExportSerializerName = exportSerializer;
            ParsingType = parsingType;
        }

        public Type ParsingType { get; }
        public string ExportSerializerName { get; }
        public string ExportNamespace { get; set; }
    }
}
