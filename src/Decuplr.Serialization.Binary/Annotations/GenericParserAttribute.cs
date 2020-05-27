using System;

namespace Decuplr.Serialization.Binary.Annotations {
    /// <summary>
    /// Describes that a parser provider that it provides is a parser of a generic type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class GenericParserAttribute : Attribute {
        public GenericParserAttribute(Type type, int genericsCount) {
            Type = type;
        }

        public Type Type { get; }
        public bool IsValid => Type.IsGenericType;
    }
}
