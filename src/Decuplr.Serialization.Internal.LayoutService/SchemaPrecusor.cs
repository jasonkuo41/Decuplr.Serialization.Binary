using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.LayoutService {
    public struct SchemaPrecusor {

        private IReadOnlyList<string>? _namespaces;

        private IReadOnlyList<ITypeSymbol>? _externalDepedencies;
        
        public bool NeverDeserialize { get; set; }

        public bool IsSealed { get; set; }

        public LayoutOrder RequestLayout { get; set; }

        public IReadOnlyList<string> TargetNamespaces {
            get => _namespaces ?? Array.Empty<string>();
            set => _namespaces = value;
        }

        /// <summary>
        /// Represents the external arguments required to create this type (e.g. InlineData). 
        /// When not empty, it cannot be a TypeParser or IParserProvider
        /// </summary>
        public IReadOnlyList<ITypeSymbol> ExternalDepedencies {
            get => _externalDepedencies ?? Array.Empty<ITypeSymbol>();
            set => _externalDepedencies = value;
        }

        // public InheritIndexRule InheritRule { get; set; }

    }

}
