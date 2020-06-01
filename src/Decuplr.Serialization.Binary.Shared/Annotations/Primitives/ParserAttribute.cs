using System;

namespace Decuplr.Serialization.Binary.Annotations {
    /// <summary>
    /// Provides a parser with the annotated type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ParserAttribute : Attribute {
        public ParserAttribute(Type type) {
            Type = type;
        }

        public Type Type { get; }
        public BinaryLayout Layout { get; set; }
    }
}
