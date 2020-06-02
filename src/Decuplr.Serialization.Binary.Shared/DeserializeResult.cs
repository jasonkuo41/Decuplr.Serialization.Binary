using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization {
    public struct DeserializeResult {



        /// <summary>
        /// The serialization was a success
        /// </summary>
        public static DeserializeResult Success { get; }
        public static DeserializeResult Faulted { get; }
        public static DeserializeResult InsufficientSize { get; }

        public static DeserializeResult FaultedFrom(string result) {

        }

    }

    public enum DeserializeConculsion {
        Success,
        Faulted,
        InsufficientSize
    }
}
