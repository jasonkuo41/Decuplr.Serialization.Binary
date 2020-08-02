using System;
using Decuplr.CodeAnalysis.Serialization.TypeComposite;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization {
    public class TypeParserInfo : IEquatable<TypeParserInfo> {
        /// <summary>
        /// The symbol representing the type parser
        /// </summary>
        public INamedTypeSymbol Parser { get; }

        /// <summary>
        /// The type composer that is responsible for creating this type
        /// </summary>
        public ITypeComposer Composer { get; }

        public TypeParserInfo(INamedTypeSymbol parser, ITypeComposer composer) {
            Parser = parser;
            Composer = composer;
        }

        public bool Equals(TypeParserInfo parserInfo) => parserInfo.Parser.Equals(Parser, SymbolEqualityComparer.Default);
        public override bool Equals(object obj) => obj is TypeParserInfo info ? Equals(info) : false;

        public override int GetHashCode() => Parser.GetHashCode();
    }
}
