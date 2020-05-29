using System.Collections.Generic;
using Decuplr.Serialization.Binary.Namespaces;

namespace Decuplr.Serialization.Binary {

    public abstract class BinaryPacker : INamespaceRoot {

        /// <inheritdoc/>
        public abstract IDefaultParserNamespace DefaultNamespace { get; }

        /// <inheritdoc/>
        public abstract IParserNamespaceOwner CreateNamespace(string parserNamespace);

        /// <inheritdoc/>
        public abstract IParserDiscovery CreateDiscovery(IEnumerable<string> parserNamespace);

        /// <inheritdoc/>
        public abstract IParserDiscovery CreateDiscovery();

        /// <summary>
        /// Get's the shared instance of the binary packer
        /// </summary>
        public static BinaryPacker Shared { get; } = new DefaultBinaryPacker();

        // Currently we won't support creating your own packer right now, since there needs some discuss on how to correctly implement that and expose correct API
        //public static BinaryPacker Create(bool includeDefaultSerializers) => new DefaultBinaryPacker(includeDefaultSerializers);
    }
}

