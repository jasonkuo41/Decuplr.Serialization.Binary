﻿using Decuplr.CodeAnalysis.Serialization.Arguments;
using Decuplr.CodeAnalysis.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite {
    /// <summary>
    /// Represents the info of each component, how to generate the initialization method and it's type name
    /// </summary>
    public interface IComponentTypeInfo : IMemberComposingMethod {
        /// <summary>
        /// The full name of the component, for example "TypeParser`T" or "ByteOrder"
        /// </summary>
        INamedTypeSymbol Type { get; }

        void ProvideInitialize(CodeNodeBuilder builder, string discoveryName);

        void ProvideTryInitialize(CodeNodeBuilder builder, string discoveryName, string isSuccessName);
    }
}
