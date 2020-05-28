using System.Collections.Generic;
using Decuplr.Serialization.Binary.Namespaces;

namespace Decuplr.Serialization.Binary {

    public abstract class BinaryPacker : INamespaceRoot {

        public abstract IDefaultParserNamespace DefaultNamespace { get; }

        public abstract IParserNamespaceSource CreateNamespace(string parserNamespace);

        public abstract IParserNamespace GetNamespace(IEnumerable<string> parserNamespace);

        public static BinaryPacker Shared { get; } = new DefaultBinaryPacker();

        // Currently we won't support creating your own packer right now, since there needs some discuss on how to correctly implement that and expose correct API
        //public static BinaryPacker Create(bool includeDefaultSerializers) => new DefaultBinaryPacker(includeDefaultSerializers);
    }
}

