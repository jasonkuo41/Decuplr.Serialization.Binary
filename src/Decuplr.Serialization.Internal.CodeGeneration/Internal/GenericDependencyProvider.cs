﻿using System.Collections.Generic;
using Decuplr.Serialization.Binary;
using Decuplr.Serialization.CodeGeneration.Arguments;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration.Internal {

    internal class GenericDependencyProvider : IDependencyProviderSource {

        private class DefaultProvider : IComponentProvider {
            public string FullTypeName { get; }

            public DefaultProvider(string fullTypeName) {
                FullTypeName = fullTypeName;
            }

            public string GetComponent(ParserDiscoveryArgs args) => $"return {args}.GetParser<{FullTypeName}>();";

            public string TryGetComponent(ParserDiscoveryArgs args, OutArgs<object> result) => $"return {args}.TryGetParser<{FullTypeName}>(out {result});";
        }

        private class PrimitiveTypeProvider : IComponentProvider {
            public string FullTypeName => "ByteOrder";

            public string GetComponent(ParserDiscoveryArgs args) => $"return {args}.ByteOrder;";

            public string TryGetComponent(ParserDiscoveryArgs args, OutArgs<object> result) => $"{result} = {args}.ByteOrder; return true;";

            public static PrimitiveTypeProvider Shared { get; } = new PrimitiveTypeProvider();
        }

        private class StringTypeProvider : IComponentProvider {
            public string FullTypeName => "Encoding";

            public string GetComponent(ParserDiscoveryArgs args) => $"return {args}.{nameof(IParserDiscovery.TextEncoding)};";

            public string TryGetComponent(ParserDiscoveryArgs args, OutArgs<object> result) => $"{result} = {args}.{nameof(IParserDiscovery.TextEncoding)}; return true;";

            public static StringTypeProvider Shared { get; } = new StringTypeProvider();
        }

        private readonly Dictionary<string, IComponentProvider> _components = new Dictionary<string, IComponentProvider>();

        public IReadOnlyDictionary<string, IComponentProvider> Components => _components;

        private static IComponentProvider GetDefault(ITypeSymbol symbol) => symbol switch
        {
            _ when symbol.SpecialType == SpecialType.System_String => StringTypeProvider.Shared,
            _ when symbol.IsPrimitiveType() => PrimitiveTypeProvider.Shared,
            _ => new DefaultProvider(symbol.ToString())
        };

        public string GetComponentName(ITypeSymbol symbol) {
            var componentName = $"component_{_components.Count}";
            _components.Add(componentName, GetDefault(symbol));
            return componentName;
        }
    }
}