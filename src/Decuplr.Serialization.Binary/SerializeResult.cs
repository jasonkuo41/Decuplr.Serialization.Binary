using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization {
    public enum SerializeResult {
        /// <summary>
        /// The serialization was a success
        /// </summary>
        Success,
        Faulted,
        InsufficientSize
    }
}
