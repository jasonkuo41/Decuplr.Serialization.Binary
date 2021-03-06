﻿using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.CodeAnalysis.Meta;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization {
    public class SchemaInfoBuilder {

        public SchemaInfoBuilder(string schemaName, NamedTypeMetaInfo sourceType, IOrderSelector orderSelector, IEnumerable<INamedTypeSymbol> targetTypes) {
            if (targetTypes is null || targetTypes.Count() == 0)
                throw new ArgumentException("A schema must always have a target type to be built to");
            SourceTypeInfo = sourceType;
            OrderSelector = orderSelector;
            SchemaName = schemaName;
            TargetTypes.AddRange(targetTypes);
        }

        public SchemaInfoBuilder(string schemaName, NamedTypeMetaInfo sourceType, IOrderSelector orderSelector, params INamedTypeSymbol[] targetTypes) 
            : this(schemaName, sourceType, orderSelector, targetTypes.AsEnumerable()) { 
        }

        /// <summary>
        /// The type that is resposible for this
        /// </summary>
        public NamedTypeMetaInfo SourceTypeInfo { get; }

        /// <summary>
        /// Indicates the parser should never be deserialized
        /// </summary>
        public bool NeverDeserialize { get; set; }

        /// <summary>
        /// Indicates the parser should not be modified by any namespace modifiers
        /// </summary>
        public bool IsSealed { get; set; }

        /// <summary>
        /// The name of the schema that this would generate
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        /// The order selector that this schema uses
        /// </summary>
        public IOrderSelector OrderSelector { get; set; }

        /// <summary>
        /// Indicates the namespaces that this type would be targeted to
        /// </summary>
        public List<string> TargetNamespaces { get; } = new List<string>();

        /// <summary>
        /// Represents the external arguments required to create this type (e.g. InlineData). 
        /// When not empty, it cannot be a TypeParser or IParserProvider
        /// </summary>
        public List<ITypeSymbol> ExternalArguments { get; } = new List<ITypeSymbol>();

        /// <summary>
        /// Represents what this schema would be used for to build parser for what types
        /// </summary>
        public List<INamedTypeSymbol> TargetTypes { get; } = new List<INamedTypeSymbol>();

        /// <summary>
        /// Create a schema info
        /// </summary>
        /// <returns>The Schema Info Instance</returns>
        public SchemaInfo CreateSchemaInfo() => new SchemaInfo(this);
    }
}
