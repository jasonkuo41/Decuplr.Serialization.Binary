using System;

namespace Decuplr.Serialization {
    /// <summary>
    /// Represents the base type for exceptions that occur during serialization or deserialization for the Decuplr.Serialization library
    /// </summary>
    public class SerializationFaultException : Exception {

        public SerializationFaultException() { }

        public SerializationFaultException(string? message) 
            : base(message) {
        }

        public SerializationFaultException(string? message, Exception? innerException) 
            : base(message, innerException) {
        }
    }

}
