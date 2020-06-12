using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary.Annotations {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public sealed class ExportPublicParserAttribute : Attribute {
        public ExportPublicParserAttribute(string exportParserName, Type parsingType) {
            ExportParserName = exportParserName;
            ParsingType = parsingType;
        }

        public Type ParsingType { get; }
        public string ExportParserName { get; }
        public string ExportNamespace { get; set; }
    }
}
