using System;
using System.Collections.Generic;
using Decuplr.Serialization.Binary;
using Decuplr.Serialization.CodeGeneration.Arguments;
using Decuplr.Serialization.CodeGeneration.TypeComposite;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration.Internal {

    [Obsolete]
    internal class GenericDependencyProvider : IDependencyProviderSource {

        private class DefaultProvider : IComponentProviderObsolete {
            public string FullTypeName { get; }

            public DefaultProvider(string fullTypeName) {
                FullTypeName = fullTypeName;
            }

            public string GetComponent(ParserDiscoveryArgs args) => $"return {args}.GetParser<{FullTypeName}>();";

            public string TryGetComponent(ParserDiscoveryArgs args, OutArgs<object> result) => $"return {args}.TryGetParser<{FullTypeName}>(out {result});";
        }

        private class PrimitiveTypeProvider : IComponentProviderObsolete {
            public string FullTypeName => "ByteOrder";

            public string GetComponent(ParserDiscoveryArgs args) => $"return {args}.ByteOrder;";

            public string TryGetComponent(ParserDiscoveryArgs args, OutArgs<object> result) => $"{result} = {args}.ByteOrder; return true;";

            public static PrimitiveTypeProvider Shared { get; } = new PrimitiveTypeProvider();
        }

        private class StringTypeProvider : IComponentProviderObsolete {
            public string FullTypeName => "Encoding";

            public string GetComponent(ParserDiscoveryArgs args) => $"return {args}.{nameof(IParserDiscovery.TextEncoding)};";

            public string TryGetComponent(ParserDiscoveryArgs args, OutArgs<object> result) => $"{result} = {args}.{nameof(IParserDiscovery.TextEncoding)}; return true;";

            public static StringTypeProvider Shared { get; } = new StringTypeProvider();
        }

        private readonly Dictionary<string, IComponentProviderObsolete> _components = new Dictionary<string, IComponentProviderObsolete>();

        public IReadOnlyDictionary<string, IComponentProviderObsolete> Components => _components;

        IReadOnlyList<ITypeSymbol> IComponentCollection.Components => throw new System.NotImplementedException();

        private static IComponentProviderObsolete GetDefault(ITypeSymbol symbol) => symbol switch
        {
            _ when symbol.SpecialType == SpecialType.System_String => StringTypeProvider.Shared,
            _ when symbol.IsPrimitiveType() => PrimitiveTypeProvider.Shared,
            _ => new DefaultProvider(symbol.ToString())
        };

        public string AddComponent(ITypeSymbol symbol) {
            var componentName = $"component_{_components.Count}";
            _components.Add(componentName, GetDefault(symbol));
            return componentName;
        }

        ComposerMethods IComponentCollection.AddComponent(ITypeSymbol symbol) {
            throw new System.NotImplementedException();
        }
    }
}
