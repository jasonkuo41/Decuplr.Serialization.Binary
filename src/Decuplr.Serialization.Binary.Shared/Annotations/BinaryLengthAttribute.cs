using System;

namespace Decuplr.Serialization.Binary.Annotations {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class BinaryLengthAttribute : Attribute {
        public BinaryLengthAttribute(int constantLength) {
            ConstantLength = constantLength;
        }

        public BinaryLengthAttribute(string lengthProvider) {
            LengthProviderName = lengthProvider;
        }

        public int? ConstantLength { get; }
        public string LengthProviderName { get; }
    }
}
