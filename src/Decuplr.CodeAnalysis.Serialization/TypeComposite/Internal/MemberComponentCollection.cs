using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite.Internal {
    internal class MemberComponentCollection : IComponentCollection {

        private readonly List<ITypeSymbol> _symbols = new List<ITypeSymbol>();

        public ComposerMethodNames GetMethodNames(int index)
            => new ComposerMethodNames {
                TryDeserializeSequence = $"TryDeserialize_Component_{index}",
                TryDeserializeSpan = $"TryDeserialize_Component_{index}",
                DeserializeSequence = $"Deserialize_Component_{index}",
                DeserializeSpan = $"Deserialize_Component_{index}",
                TrySerialize = $"TrySerialize_Component_{index}",
                Serialize = $"Serialize_Component_{index}",
                GetLength = $"GetLength_Component_{index}",
            };

        public IReadOnlyList<ITypeSymbol> Components => _symbols;

        public ComposerMethods AddComponent(ITypeSymbol symbol) {
            _symbols.Add(symbol);
            return new ComposerMethods(GetMethodNames(_symbols.Count - 1));
        }
    }
}
