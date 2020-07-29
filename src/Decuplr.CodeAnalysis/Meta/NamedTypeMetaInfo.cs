using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Decuplr.CodeAnalysis.Meta.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.CodeAnalysis.Meta {

    public class NamedTypeMetaInfo : ISymbolMetaInfo<INamedTypeSymbol>, IEquatable<NamedTypeMetaInfo> {

        private readonly ITypeSymbolProvider _analysis;

        public bool IsPartial { get; }

        public INamedTypeSymbol Symbol { get; }

        public IReadOnlyList<MemberMetaInfo> Members { get; }

        public IReadOnlyList<IReadOnlyList<AttributeData>> Attributes { get; }

        public IReadOnlyList<TypePartialMetaInfo> Declarations { get; }

        public Location FirstLocation { get; }

        public NamedTypeMetaInfo? GenericDefinition { get; }

        ITypeSymbolProvider ISymbolMetaInfo<INamedTypeSymbol>.SymbolProvider => _analysis;

        internal NamedTypeMetaInfo(ITypeSymbolProvider analysis, INamedTypeSymbol symbol, IEnumerable<SyntaxModelPair> syntax, Func<ISymbol, bool>? memberPredicate, CancellationToken ct) {
            _analysis = analysis;

            Symbol = symbol;
            IsPartial = syntax.Any(x => x.Syntax.Modifiers.Any(SyntaxKind.PartialKeyword));
            FirstLocation = syntax.First().Syntax.GetLocation();
            Attributes = Declarations.SelectMany(x => x.Attributes).ToList();

            Declarations = syntax.Select(x => new TypePartialMetaInfo(this, analysis, x, memberPredicate, ct)).ToList();
            Members = Declarations.SelectMany(x => x.Members).ToList();
        }

        private NamedTypeMetaInfo(NamedTypeMetaInfo source, INamedTypeSymbol newSymbol) {
            _analysis = source._analysis;

            Symbol = newSymbol;
            IsPartial = source.IsPartial;
            FirstLocation = source.FirstLocation;
            Attributes = source.Attributes;

            Declarations = source.Declarations.Select(x => new TypePartialMetaInfo(this, x, newSymbol)).ToList();
            Members = Declarations.SelectMany(x => x.Members).ToList();

            GenericDefinition = source;
        }

        public NamedTypeMetaInfo MakeGenericType(params ITypeSymbol[] symbols) {
            if (!Symbol.IsUnboundGenericType)
                throw new InvalidOperationException($"{Symbol} is not a unbound generic type.");
            return new NamedTypeMetaInfo(this, Symbol.Construct(symbols));
        }

        public bool ContainsAttribute<TAttribute>() where TAttribute : Attribute {
            var symbol = _analysis.GetSymbol<TAttribute>();
            if (symbol is null)
                return false;
            return Attributes.SelectMany(x => x).Any(x => x.AttributeClass?.Equals(symbol, SymbolEqualityComparer.Default) ?? false);
        }

        public IEnumerable<AttributeData> GetAttributes<TAttribute>() where TAttribute : Attribute {
            var symbol = _analysis.GetSymbol<TAttribute>();
            if (symbol is null)
                return Enumerable.Empty<AttributeData>();
            return Attributes.SelectMany(x => x).Where(x => x.AttributeClass?.Equals(symbol, SymbolEqualityComparer.Default) ?? false);
        }

        public IEnumerable<TypePartialMetaInfo> ContainingLocations(AttributeData attributeData) {
            return Declarations.Where(x => x.Attributes.Equals(attributeData));
        }

        public override string ToString() => Symbol.ToString();
        public bool Equals(NamedTypeMetaInfo other) => other.Symbol.Equals(Symbol, SymbolEqualityComparer.Default);
        public override bool Equals(object obj) => obj is NamedTypeMetaInfo symbol && Equals(symbol);
        public override int GetHashCode() => Symbol.GetHashCode();

    }
}
