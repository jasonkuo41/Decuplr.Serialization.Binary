using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization {
    /// <summary>
    /// Represents the base type for exceptions that occur during serialization or deserialization for the Decuplr.Serialization library
    /// </summary>
    public class SerializationFaultException : Exception {

        public SerializationFaultException() { }

        public SerializationFaultException(string message) 
            : base(message) {
        }

        public SerializationFaultException(string message, Exception innerException) 
            : base(message, innerException) {
        }
    }

    /// <summary>
    /// Indicates that the parser of a certain type is not found
    /// </summary>
    public class SerializerNotFoundException : SerializationFaultException {

        public SerializerNotFoundException() { }

        public SerializerNotFoundException(string message) 
            : base(message) {
        }

        public SerializerNotFoundException(string message, Type parsedType) 
            : base(message) {
            ParsedType = parsedType;
        }

        public SerializerNotFoundException(string message, Exception innerException) 
            : base(message, innerException) {
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

    /// <summary>
    /// Indicates that a circular reference of another schema was detected, thus the serializer cannot be created
    /// </summary>
    public class CircularSchemaReferenceException : SerializationFaultException {

        public CircularSchemaReferenceException() { }

        public CircularSchemaReferenceException(string message) : base(message) {
        }

        public CircularSchemaReferenceException(string message, Type circularType) : base(message) {
            CircularSchemaType = circularType;
        }

        public CircularSchemaReferenceException(string message, Exception innerException) : base(message, innerException) {
        }

        public CircularSchemaReferenceException(string message, Type circularType, Exception innerException) : base(message, innerException) {
            CircularSchemaType = circularType;
        }

        public Type CircularSchemaType { get; }
    }
}
