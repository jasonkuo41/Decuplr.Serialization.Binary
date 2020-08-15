using System;
using System.Collections.Generic;
using Decuplr.Serialization.Namespaces;

namespace Decuplr.Serialization.Binary {
    public class BinarySerializerOptions {

        public static BinarySerializerOptions Default { get; }

        public static BinarySerializerOptions Performance { get; }

        public static IReadOnlyNamespaceContainer DefaultContainer { get; }

        static BinarySerializerOptions() {

        }

        public CircularReferenceMode CircularReferenceMode { get; set; }

        public IReadOnlyNamespaceContainer CurrentContainer { get; set; }


    }
}
