using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization.Internal {

    internal class SchemaCompilationInfo : ISchemaCompilationInfo {

        private readonly Dictionary<INamedTypeSymbol, List<ISchemaFactory>> symbols = new Dictionary<INamedTypeSymbol, List<ISchemaFactory>>();

        public IReadOnlyList<ISchemaFactory> CompilingSchemas { get; }

        public SchemaCompilationInfo(IEnumerable<ISchemaFactory> schemaFactories) {
            CompilingSchemas = schemaFactories.ToList();

            foreach(var schema in CompilingSchemas) {
                var symbol = schema.Layout.Type.Symbol;
                if (symbols.TryGetValue(symbol, out var list))
                    list.Add(schema);
                else
                    symbols[symbol] = new List<ISchemaFactory> { schema };
            }
        }

        public IEnumerable<ISchemaFactory> GetSchemaComponents(INamedTypeSymbol symbol) => symbols[symbol];
    }
}
