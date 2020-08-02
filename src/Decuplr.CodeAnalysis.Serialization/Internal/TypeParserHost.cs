using System.Collections.Generic;
using Decuplr.CodeAnalysis.Serialization.TypeComposite;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization.Internal {
    internal class TypeParserHost {
        private class TypeParserBuilder : ITypeParserBuilder {

            private readonly TypeParserHost _host;
            private readonly IParsingSolution _solution;

            public SchemaLayout Layout { get; }

            public TypeParserBuilder(SchemaLayout layout, IParsingSolution solution, TypeParserHost host) {
                _host = host;
                _solution = solution;
                Layout = layout;
            }

            public TypeParserInfo CreateParser(IComponentProvider provider, string uniqueName) {
                var parser = _solution.CreateParser(Layout, provider, uniqueName);
                _host._parsers.Add(parser);
                return parser;
            }

            public ITypeParserBuilder MakeGenericType(params ITypeSymbol[] symbols) => new TypeParserBuilder(Layout.MakeGenericType(symbols), _solution, _host);
        }

        private readonly List<TypeParserInfo> _parsers = new List<TypeParserInfo>();

        public IReadOnlyList<TypeParserInfo> GeneratedParsers => _parsers;

        public ITypeParserBuilder CreateBuilder(SchemaLayout layout, IParsingSolution solution) => new TypeParserBuilder(layout, solution, this);

    }
}
