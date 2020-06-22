using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.Serialization.Binary.Analyzers;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.AnalyzeService {
    class SourceCodeAnalysis {

        private readonly Compilation Compilation;
        private readonly Dictionary<Type, INamedTypeSymbol?> CachedSymbols = new Dictionary<Type, INamedTypeSymbol?>();

        public SourceCodeAnalysis(Compilation compilation) {
            Compilation = compilation;
        }

        public INamedTypeSymbol? GetSymbol<T>() => GetSymbol(typeof(T));

        public INamedTypeSymbol? GetSymbol(Type type) {
            if (CachedSymbols.TryGetValue(type, out var symbol))
                return symbol;
            symbol = Compilation.GetTypeByMetadataName(type.FullName);
            CachedSymbols.Add(type, symbol);
            return symbol;
        }


    }

    class TypeMetaInfo {
        public bool IsPartial { get; }

        public INamedTypeSymbol Symbol { get; }

        public IReadOnlyList<MemberMetaInfo> Members { get; }

        public IReadOnlyList<AnalyzedPartialType> Declartions { get; }
    }

    class MemberMetaInfo {

    }
}
