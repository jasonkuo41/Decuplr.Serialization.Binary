using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.CodeAnalysis.Meta;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization {

    public class SchemaInfo {

        /// <summary>
        /// The source type that is responsible for producing this schema info
        /// </summary>
        public NamedTypeMetaInfo SourceTypeInfo { get; private set; }

        /// <summary>
        /// Indicates the parser should never be deserialized
        /// </summary>
        public bool NeverDeserialize { get; }

        /// <summary>
        /// Indicates the parser should not be modified by any namespace modifiers
        /// </summary>
        public bool IsSealed { get; }

        /// <summary>
        /// The name of the schema that this would generate
        /// </summary>
        public string SchemaName { get; }

        /// <summary>
        /// Indicates the namespaces that this type would be targeted to
        /// </summary>
        public IReadOnlyList<string> TargetNamespaces { get; }

        /// <summary>
        /// The order selector that this schema uses
        /// </summary>
        public IOrderSelector OrderSelector { get; }

        /// <summary>
        /// Represents the external arguments required to create this type (e.g. InlineData). 
        /// When not empty, it cannot be a TypeParser or IParserProvider
        /// </summary>
        public IReadOnlyList<ITypeSymbol> ExternalArguments { get; }

        /// <summary>
        /// Represents what this schema would be used for to build parser for what types
        /// </summary>
        public IReadOnlyList<INamedTypeSymbol> TargetTypes { get; }

        public static SchemaInfoBuilder CreateBuilder(string schemaName, NamedTypeMetaInfo type, IOrderSelector orderSelector, params INamedTypeSymbol[] targetTypes) => new SchemaInfoBuilder(schemaName, type, orderSelector, targetTypes);
        public static SchemaInfoBuilder CreateBuilder(string schemaName, NamedTypeMetaInfo type, IOrderSelector orderSelector, IEnumerable<INamedTypeSymbol> targetTypes) => new SchemaInfoBuilder(schemaName, type, orderSelector, targetTypes);

        internal SchemaInfo(SchemaInfoBuilder builder) {
            SourceTypeInfo = builder.SourceTypeInfo;
            NeverDeserialize = builder.NeverDeserialize;
            IsSealed = builder.IsSealed;
            SchemaName = builder.SchemaName;
            OrderSelector = builder.OrderSelector;
            TargetNamespaces = builder.TargetNamespaces.ToList();
            ExternalArguments = builder.ExternalArguments.ToList();
            TargetTypes = builder.TargetTypes.ToList();
        }

        public SchemaInfo MakeGenericType(params ITypeSymbol[] symbols) {
            var schemaInfo = (SchemaInfo)MemberwiseClone();
            schemaInfo.SourceTypeInfo = SourceTypeInfo.MakeGenericType(symbols);
            return schemaInfo;
        }
    }
}
