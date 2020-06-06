﻿using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.Serialization.Binary.Namespaces;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    internal interface IParserKindProvider {
        string GetFunction(string namespaceName, string rootName);
    }

    internal class SealedParserKindProvider : IParserKindProvider {
        private readonly string ParserName;
        private readonly bool AcceptsRoot;

        public SealedParserKindProvider(string parserName, bool acceptsRoot) {
            ParserName = parserName;
            AcceptsRoot = acceptsRoot;
        }

        public string GetFunction(string namespaceName, string rootName) {
            return $"{namespaceName}.{nameof(IMutableNamespace.AddSealedParser)}(new {ParserName}({(AcceptsRoot ? rootName : null)}))";
        }
    }

    internal class ParserProviderKindProvider : IParserKindProvider {

        private readonly INamedTypeSymbol ParsedType;
        private readonly string ParserProviderName;

        public ParserProviderKindProvider(INamedTypeSymbol parsedType, string parserProviderName) {
            ParserProviderName = parserProviderName;
            ParsedType = parsedType;
        }

        // bool AddParserProvider<TProvider, TType>(TProvider provider) where TProvider : IParserProvider<TType>;
        public string GetFunction(string namespaceName, string rootName) {
            return $"{namespaceName}.{nameof(IMutableNamespace.AddParserProvider)}<{ParserProviderName}, {ParsedType}>(new {ParserProviderName}())";
        }
    }

    internal class GenericParserKindProvider : IParserKindProvider {
        private readonly INamedTypeSymbol ParsedType;
        private readonly string ParserProviderName;

        public GenericParserKindProvider(INamedTypeSymbol parsedType, string parserProviderName) {
            ParsedType = parsedType;
            ParserProviderName = parserProviderName;
        }

        public string GetFunction(string namespaceName, string rootName) {
            return $"{namespaceName}.{nameof(IDefaultParserNamespace.AddGenericParserProvider)}({ParserProviderName}, typeof({ParsedType}).{nameof(Type.GetGenericTypeDefinition)}())";
        }
    }
}