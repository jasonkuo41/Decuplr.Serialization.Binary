using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary {

    internal class ImplBinarySerializer : BinarySerializer {

    }

    public abstract class BinarySerializer {

        public static BinarySerializer Shared { get; } = new ImplBinarySerializer();
    }
}
