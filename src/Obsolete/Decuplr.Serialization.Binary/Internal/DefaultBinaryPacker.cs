using System;
using System.Collections.Generic;
using Decuplr.Serialization.Binary.Internal;
using Decuplr.Serialization.Binary.Namespaces;

namespace Decuplr.Serialization.Binary {

    internal partial class DefaultBinaryPacker : BinaryPacker {

        private readonly ParserNamespaces Namespaces;

        public override IDefaultParserNamespace DefaultNamespace => Namespaces.Default;

        public DefaultBinaryPacker() {
            Namespaces = new ParserNamespaces(this);

            // TODO : [Prioritzed] Note this would absolutely result in a but, that says we can't create namespaces that starts with an Internal prefix
            new DefaultParserEntryPoint().LoadContext(this);
        }

        public override IParserDiscovery CreateDiscovery() => new ParserDiscovery(Namespaces);
        public override IParserDiscovery CreateDiscovery(IEnumerable<string> parserNamespace) => new ParserDiscovery(Namespaces, parserNamespace);

        public override IParserNamespaceOwner CreateNamespace(string parserNamespace) {
            if (parserNamespace.StartsWith("Internal."))
                throw new ArgumentException("Cannot create namespaces that starts with an `Internal` prefix", nameof(parserNamespace));
            if (Namespaces.TryAddNamespace(parserNamespace, out var container))
                throw new ArgumentException("The requested namespace already exists", nameof(parserNamespace));
            return container;
        }

    }
}
