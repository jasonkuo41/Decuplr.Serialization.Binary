using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.Serialization.Binary.AnalysisService {

    internal class BaseTypeMetaInfo {

        private readonly SourceCodeAnalysis Analysis;

        public bool IsPartial { get; }

        public string UniqueName { get; }

        public INamedTypeSymbol Symbol { get; }

        public IReadOnlyList<MemberMetaInfo> Members { get; }

        public IReadOnlyList<IReadOnlyList<AttributeData>> Attributes { get; }

        public IReadOnlyList<TypePartialMetaInfo> Declarations { get; }

        public Location FirstLocation { get; }

        internal BaseTypeMetaInfo(SourceCodeAnalysis analysis, INamedTypeSymbol symbol, IEnumerable<SyntaxModelPair> syntax, IReadOnlyList<SymbolKind> kinds, CancellationToken ct) {
            IsPartial = syntax.Any(x => x.Syntax.Modifiers.Any(SyntaxKind.PartialKeyword));
            Analysis = analysis;
            Symbol = symbol;
            UniqueName = symbol.GetUniqueLegalName();
            Declarations = syntax.Select(x => new TypePartialMetaInfo(this, x, kinds, ct)).ToList();
            FirstLocation = syntax.First().Syntax.GetLocation();
            Attributes = Declarations.SelectMany(x => x.Attributes).ToList();
            Members = Declarations.SelectMany(x => x.Members).ToList();
        }

        public bool ContainsAttribute<TAttribute>() where TAttribute : Attribute {
            var symbol = Analysis.GetSymbol<TAttribute>();
            if (symbol is null)
                return false;
            return Attributes.SelectMany(x => x).Any(x => x.AttributeClass?.Equals(symbol, SymbolEqualityComparer.Default) ?? false);
        }

        public IEnumerable<AttributeData> GetAttributes<TAttribute>() where TAttribute : Attribute {
            var symbol = Analysis.GetSymbol<TAttribute>();
            if (symbol is null)
                return Enumerable.Empty<AttributeData>();
            return Attributes.SelectMany(x => x).Where(x => x.AttributeClass?.Equals(symbol, SymbolEqualityComparer.Default) ?? false);
        }

        public IEnumerable<TypePartialMetaInfo> ContainingLocations(AttributeData attributeData) {
            return Declarations.Where(x => x.Attributes.Equals(attributeData));
        }

        public bool InheritFrom(ITypeSymbol baseType) {
            var symbol = Symbol;
            while (symbol.BaseType != null) {
                if (symbol.BaseType.Equals(baseType, SymbolEqualityComparer.Default))
                    return true;
                symbol = symbol.BaseType;
            }
            return false;
        }
        public bool InheritFrom<T>() => InheritFrom(Analysis.GetSymbol<T>()!);

        public bool Implements(INamedTypeSymbol interfaceType) => Symbol.AllInterfaces.Any(x => x.Equals(interfaceType, SymbolEqualityComparer.Default));
        public bool Implements<T>() => Implements(Analysis.GetSymbol<T>()!);
        public bool Implements(Type interfaceType) => Implements(Analysis.GetSymbol(interfaceType)!);

        public override string ToString() => UniqueName;
    }

    internal class NamedTypeMetaInfo {

        private readonly SourceCodeAnalysis Analysis;

        public bool IsPartial { get; }

        public string UniqueName { get; }

        public INamedTypeSymbol Symbol { get; }

        public IReadOnlyList<MemberMetaInfo> Members { get; }

        public IReadOnlyList<IReadOnlyList<AttributeData>> Attributes { get; }

        public IReadOnlyList<TypePartialMetaInfo> Declarations { get; }

        public Location FirstLocation { get; }

        internal NamedTypeMetaInfo(SourceCodeAnalysis analysis, INamedTypeSymbol symbol, IEnumerable<SyntaxModelPair> syntax, IReadOnlyList<SymbolKind> kinds, CancellationToken ct) {
            IsPartial = syntax.Any(x => x.Syntax.Modifiers.Any(SyntaxKind.PartialKeyword));
            Analysis = analysis;
            Symbol = symbol;
            UniqueName = symbol.GetUniqueLegalName();
            Declarations = syntax.Select(x => new TypePartialMetaInfo(this, x, kinds, ct)).ToList();
            FirstLocation = syntax.First().Syntax.GetLocation();
            Attributes = Declarations.SelectMany(x => x.Attributes).ToList();
            Members = Declarations.SelectMany(x => x.Members).ToList();
        }

        public bool ContainsAttribute<TAttribute>() where TAttribute : Attribute {
            var symbol = Analysis.GetSymbol<TAttribute>();
            if (symbol is null)
                return false;
            return Attributes.SelectMany(x => x).Any(x => x.AttributeClass?.Equals(symbol, SymbolEqualityComparer.Default) ?? false);
        }

        public IEnumerable<AttributeData> GetAttributes<TAttribute>() where TAttribute : Attribute {
            var symbol = Analysis.GetSymbol<TAttribute>();
            if (symbol is null)
                return Enumerable.Empty<AttributeData>();
            return Attributes.SelectMany(x => x).Where(x => x.AttributeClass?.Equals(symbol, SymbolEqualityComparer.Default) ?? false);
        }

        public IEnumerable<TypePartialMetaInfo> ContainingLocations(AttributeData attributeData) {
            return Declarations.Where(x => x.Attributes.Equals(attributeData));
        }

        public bool InheritFrom(ITypeSymbol baseType) {
            var symbol = Symbol;
            while (symbol.BaseType != null) {
                if (symbol.BaseType.Equals(baseType, SymbolEqualityComparer.Default))
                    return true;
                symbol = symbol.BaseType;
            }
            return false;
        }
        public bool InheritFrom<T>() => InheritFrom(Analysis.GetSymbol<T>()!);

        public bool Implements(INamedTypeSymbol interfaceType) => Symbol.AllInterfaces.Any(x => x.Equals(interfaceType, SymbolEqualityComparer.Default));
        public bool Implements<T>() => Implements(Analysis.GetSymbol<T>()!);
        public bool Implements(Type interfaceType) => Implements(Analysis.GetSymbol(interfaceType)!);

        public override string ToString() => UniqueName;

    }
}
