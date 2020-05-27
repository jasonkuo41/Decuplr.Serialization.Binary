using System;
using System.Runtime.CompilerServices;

namespace Decuplr.Serialization.Binary.Internal {
    internal abstract class UnmanagedParserBase<T> : TypeParser<T> where T : unmanaged {

        private readonly static int fixedSize = Unsafe.SizeOf<T>();

        public override int? FixedSize => fixedSize;
        public override int GetBinaryLength(T value) => fixedSize;

        protected abstract T GetValue(ReadOnlySpan<byte> value);
        protected abstract void WriteBytes(Span<byte> destination, T value);

        public override DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out T result) {
            readBytes = -1;
            result = default;
            if (span.Length < fixedSize)
                return DeserializeResult.InsufficientSize;
            readBytes = fixedSize;
            result = GetValue(span);
            return DeserializeResult.Success;
        }

        public override bool TrySerialize(T value, Span<byte> destination, out int writtenBytes) {
            writtenBytes = -1;
            if (destination.Length < FixedSize.Value)
                return false;
            writtenBytes = fixedSize;
            WriteBytes(destination, value);
            return true;
        }
    }
}
