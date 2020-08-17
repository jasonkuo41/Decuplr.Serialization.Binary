using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary {
    public class BinarySerializationException : SerializationFaultException {
        public BinarySerializationException() {
        }

        public BinarySerializationException(string? message) : base(message) {
        }

        public BinarySerializationException(string? message, Exception? innerException) : base(message, innerException) {
        }


    }
}
