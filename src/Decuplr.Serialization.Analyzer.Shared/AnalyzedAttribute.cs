using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.Analyzers
{
    public struct AnalyzedAttribute {
        public AttributeData Data { get; set; }
        public Location Location { get; set; }
        public bool IsEmpty => Data is null;

        public static implicit operator AttributeData(AnalyzedAttribute attribute) => attribute.Data;
    }
}
