using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization {

    /// <summary>
    /// Indicates that the parser of a certain type is not found
    /// </summary>
    public class SerializerNotFoundException : SerializationFaultException {

        public SerializerNotFoundException(string message, Type parsedType) 
            : base(message) {
            ParsedType = parsedType;
        }

        public SerializerNotFoundException(string message, Type parsedType, Exception innerException)
            : base(message, innerException) {
            ParsedType = parsedType;
        }

        /// <summary>
        /// The type attempt to be parsed
        /// </summary>
        public Type ParsedType { get; }
    }

}
