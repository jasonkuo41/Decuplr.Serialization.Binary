using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg {
    internal class ReflectionTypeSymbolLocator {

        private readonly Compilation _compilation;
        private readonly Dictionary<Type, IAssemblySymbol?> _typeAssemblyCache = new Dictionary<Type, IAssemblySymbol?>();
        private readonly Dictionary<Type, ITypeSymbol?> _typeSymbolCache = new Dictionary<Type, ITypeSymbol?>();

        public ReflectionTypeSymbolLocator(Compilation compilation) {
            _compilation = compilation;
        }

        private ITypeSymbol? GetArraySymbol(Type type) {
            Debug.Assert(type.IsArray);
            var elementSymbol = GetTypeSymbol(type.GetElementType());
            var rank = type.GetArrayRank();
            if (elementSymbol is null)
                return null;
            return _compilation.CreateArrayTypeSymbol(elementSymbol, rank);
        }

        private ITypeSymbol? GetPointerSymbol(Type type) {
            Debug.Assert(type.IsPointer);
            var elementSymbol = GetTypeSymbol(type.GetElementType());
            if (elementSymbol is null)
                return null;
            return _compilation.CreatePointerTypeSymbol(elementSymbol);
        }

        private ITypeSymbol? GetGenericNonDefinitionType(Type type) {
            Debug.Assert(type.IsGenericType);
            Debug.Assert(!type.IsGenericTypeDefinition);
            var allTypes = type.GetAllDeclaringTypes().ToList();
            var arities = type.GetArities();
            allTypes.Add(type);

            var headType = allTypes[0].IsGenericType ? allTypes[0].GetGenericTypeDefinition() : allTypes[0];
            if (!(GetTypeSymbol(headType) is INamedTypeSymbol symbol))
                return null;

            for (var i = 0; i < allTypes.Count; ++i) {
                if (symbol.TypeParameters.Length > 0) {
                    Debug.Assert(symbol.TypeParameters.Length == arities[i]);
                    var arguments = type.GenericTypeArguments.TakeLast(arities[i]).Select(x => GetTypeSymbol(x)).ToArray();
                    if (arguments.Any(x => x is null))
                        return null;
                    symbol = symbol.Construct(arguments!);
                }
                if (i == allTypes.Count - 1)
                    break;
                symbol = symbol.GetTypeMembers(allTypes[i + 1].Name, arities[i + 1]).First();
            }
            return symbol;
        }

        private ITypeSymbol? GetTypeSymbolCore(Type type) => type switch
        {
            _ when type.IsByRef => null,
            _ when type.IsArray => GetArraySymbol(type),
            _ when type.IsPointer => GetPointerSymbol(type),
            _ when type.IsGenericType && !type.IsGenericTypeDefinition => GetGenericNonDefinitionType(type),
            _ => GetAssemblySymbol(type)?.GetTypeByMetadataName(type.FullName)
        };

        public IAssemblySymbol? GetAssemblySymbol(Type type) {
            if (_typeAssemblyCache.TryGetValue(type, out var symbol))
                return symbol;

            var typeAssemblyId = AssemblyIdentity.FromAssemblyDefinition(type.Assembly);
            var queryResult = _compilation.References.Select(reference => _compilation.GetAssemblyOrModuleSymbol(reference))
                                                 .FirstOrDefault(x => x is IAssemblySymbol asmSymbol && asmSymbol.Identity.Equals(typeAssemblyId));
            var assembly = queryResult as IAssemblySymbol;
            _typeAssemblyCache[type] = assembly;
            return assembly;
        }

        public ITypeSymbol? GetTypeSymbol(Type type) {
            if (_typeSymbolCache.TryGetValue(type, out var symbol))
                return symbol;
            symbol = GetTypeSymbolCore(type);
            _typeSymbolCache[type] = symbol;
            return symbol;
        }

    }
}
