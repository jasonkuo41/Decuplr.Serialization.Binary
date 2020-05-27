using System;

namespace Decuplr.Serialization.Binary.Annotations {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class LengthOfAttribute : Attribute {
        public LengthOfAttribute(int index) {
            Index = index;
        }

        public int Index { get; }
    }
}
