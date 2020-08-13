using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary.Annotations.Internal {

    /// <summary>
    /// Defines the default parser library for Decuplr.Serialization.Binary, this attribute is only presented to source generator.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    internal sealed class DefaultParserAssemblyAttribute : Attribute { 
        public DefaultParserAssemblyAttribute(Type parserEntryType) {
            EntryType = parserEntryType;
        }

        public Type EntryType { get; }
    }

}
