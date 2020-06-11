using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary.Internal {
    internal sealed class PrimitiveParserTemplateAttribute : Attribute {
        public PrimitiveParserTemplateAttribute(Type substitute) {

        }

        public string ApplySpecialWith { get; set; }
        public object[] SpecialTypes { get; set; }
    }
}
