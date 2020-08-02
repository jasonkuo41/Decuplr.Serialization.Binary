using System.Collections.Generic;
using Decuplr.CodeAnalysis.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite.Internal {
    internal class TypeComposer : ITypeComposer {

        private readonly Dictionary<string, IMemberComposer> _memberComposers = new Dictionary<string, IMemberComposer>();

        public SchemaLayout SourceSchema { get; }

        public ITypeSymbol ComposerSymbol { get; }

        public IReadOnlyDictionary<string, IMemberComposer> MemberComposers => _memberComposers;

        public IReadOnlyList<MethodSignature> Methods { get; }

        public TypeComposer(SchemaLayout sourceSchema, ITypeSymbol composerSymbol, IReadOnlyList<MethodSignature> methods) {
            SourceSchema = sourceSchema;
            ComposerSymbol = composerSymbol;
            Methods = methods;
        }

        public void Add(string propertyName, IMemberComposer composer) {
            _memberComposers.Add(propertyName, composer);
        }
    }
}