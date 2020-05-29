using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// Represents the base type for all exceptions for the Decuplr.Serialization.Binary namespace
    /// </summary>
    public class BinarySerializationException : Exception {

        public BinarySerializationException() { }

        public BinarySerializationException(string message) 
            : base(message) {
        }

        public BinarySerializationException(string message, Exception innerException) 
            : base(message, innerException) {
        }
    }

    /// <summary>
    /// Indicates that the parser of a certain type is not found
    /// </summary>
    public class ParserNotFoundException : BinarySerializationException {

        public ParserNotFoundException() { }

        public ParserNotFoundException(string message) 
            : base(message) {
        }

        public ParserNotFoundException(string message, Type parsedType) 
            : base(message) {
            ParsedType = parsedType;
        }

        public ParserNotFoundException(string message, Exception innerException) 
            : base(message, innerException) {
        }

        public ParserNotFoundException(string message, Type parsedType, Exception innerException)
            : base(message, innerException) {
            ParsedType = parsedType;
        }

        /// <summary>
        /// The type attempt to be parsed
        /// </summary>
        public Type ParsedType { get; }
    }

    /// <summary>
    /// Indicates that a circular reference of antoher schema was detected, thus the parser can be created
    /// </summary>
    public class CircularSchemaReferenceException : BinarySerializationException {

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
