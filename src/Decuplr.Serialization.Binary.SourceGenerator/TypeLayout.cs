using System;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    internal struct TypeLayout {
        public int Index { get; set; }
        public ISymbol TargetSymbol { get; set; }
        public ISymbol TargetType { get; set; }
        public int OutputSize { get; set; }
        public int MinSize { get; set; }
        public int MaxSize { get; set; }
        public IOverrideParserAttribute? OverrideParser { get; set; }
    }
}
