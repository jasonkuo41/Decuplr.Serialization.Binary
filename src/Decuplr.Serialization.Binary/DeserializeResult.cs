using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization {
    public enum DeserializeResult {
        /// <summary>
        /// The serialization was a success
        /// </summary>
        Success,
        Faulted,
        InsufficientSize
    }
}
