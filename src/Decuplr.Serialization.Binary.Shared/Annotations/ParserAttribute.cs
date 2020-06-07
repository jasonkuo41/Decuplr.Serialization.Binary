using System;

namespace Decuplr.Serialization.Binary.Annotations {
    /// <summary>
    /// Provides a parser with the annotated type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class BinaryParserAttribute : Attribute {
        public BinaryParserAttribute(params Type[] type) {
            ParsingType = type;
        }

        public Type[] ParsingType { get; }
        public BinaryLayout Layout { get; set; }
        public bool Sealed { get; set; }
    }
}
