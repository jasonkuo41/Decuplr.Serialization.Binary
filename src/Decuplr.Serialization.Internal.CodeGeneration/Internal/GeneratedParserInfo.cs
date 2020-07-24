using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.CodeGeneration {
    public readonly struct GeneratedParserInfo {
        public GeneratedParserInfo(string name, string fullName, IReadOnlyList<ITypeSymbol> consumingArguments) {
            Name = name;
            FullName = fullName;
            ConsumingArguments = consumingArguments;
        }

        public string Name { get; }
        public string FullName { get; }
        public IReadOnlyList<ITypeSymbol> ConsumingArguments { get; }
    }
}
