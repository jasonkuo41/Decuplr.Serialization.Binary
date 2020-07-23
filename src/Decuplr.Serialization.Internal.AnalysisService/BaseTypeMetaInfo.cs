using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.AnalysisService {
    public abstract class BaseTypeMetaInfo<TSymbol> where TSymbol : ITypeSymbol {

        private readonly SourceCodeAnalysis _analysis;

        public TSymbol Symbol { get; }

        public BaseTypeMetaInfo(SourceCodeAnalysis analysis, TSymbol symbol) {
            _analysis = analysis;
            Symbol = symbol;
        }

        public IEnumerable<INamedTypeSymbol> GetInterfaces(Type type) {
            if (!type.IsInterface)
                throw new ArgumentException(nameof(type));
            var typeSymbol = _analysis.GetSymbol(type);
            if (type.IsGenericTypeDefinition)
                return Symbol.AllInterfaces.Where(x => x.IsGenericType || x.IsUnboundGenericType).Where(x => {
                    if (x.IsUnboundGenericType)
                        return x.Equals(typeSymbol, SymbolEqualityComparer.Default);
                    return x.ConstructUnboundGenericType().Equals(typeSymbol, SymbolEqualityComparer.Default);
                });
            return Symbol.AllInterfaces.Where(x => x.Equals(typeSymbol, SymbolEqualityComparer.Default));
        }

        public bool Implements(Type type) => GetInterfaces(type).Any();
        public bool Implements<T>() => Implements(typeof(T));
        public bool InheritFrom(ITypeSymbol baseType) {
            ITypeSymbol symbol = Symbol;
            while (symbol.BaseType != null) {
                if (symbol.BaseType.Equals(baseType, SymbolEqualityComparer.Default))
                    return true;
                symbol = symbol.BaseType;
            }
            return false;
        }
        public bool InheritFrom<T>() => InheritFrom(_analysis.GetSymbol<T>()!);

        public override string ToString() => Symbol.ToString();

    }
}
