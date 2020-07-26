using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Decuplr.CodeAnalysis.Meta.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.CodeAnalysis.Meta {

    public class NamedTypeMetaInfo : BaseTypeMetaInfo<INamedTypeSymbol>, IEquatable<NamedTypeMetaInfo> {

        private readonly ITypeSymbolProvider _analysis;
        private readonly IEnumerable<SyntaxModelPair> _syntax;
        private readonly Func<ISymbol, bool>? _memberPredicate;
        private readonly CancellationToken _ct;

        public bool IsPartial { get; }

        public IReadOnlyList<MemberMetaInfo> Members { get; }

        public IReadOnlyList<IReadOnlyList<AttributeData>> Attributes { get; }

        public IReadOnlyList<TypePartialMetaInfo> Declarations { get; }

        public Location FirstLocation { get; }

        internal NamedTypeMetaInfo(ITypeSymbolProvider analysis, INamedTypeSymbol symbol, IEnumerable<SyntaxModelPair> syntax, Func<ISymbol, bool>? memberPredicate, CancellationToken ct)
            : base(analysis, symbol) {
            _analysis = analysis;
            _syntax = syntax;
            _memberPredicate = memberPredicate;
            _ct = ct;

            IsPartial = syntax.Any(x => x.Syntax.Modifiers.Any(SyntaxKind.PartialKeyword));
            Declarations = syntax.Select(x => new TypePartialMetaInfo(this, analysis, x, memberPredicate, ct)).ToList();
            FirstLocation = syntax.First().Syntax.GetLocation();
            Attributes = Declarations.SelectMany(x => x.Attributes).ToList();
            Members = Declarations.SelectMany(x => x.Members).ToList();
        }

        public NamedTypeMetaInfo MakeGenericType(params ITypeSymbol[] symbols) {
            if (!Symbol.IsUnboundGenericType)
                throw new InvalidOperationException($"{Symbol} is not a unbound generic type.");
            return new NamedTypeMetaInfo(_analysis, Symbol.Construct(symbols), _syntax, _memberPredicate, _ct);
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
