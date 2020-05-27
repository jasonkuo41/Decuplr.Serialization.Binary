using System;
using System.Diagnostics;

namespace Decuplr.Serialization.Binary.Internal {
    // When T is no longer a generic type, we can finally invoke this
    internal class TypeSpanParser<T> : SpanParser<T> {
        
        private readonly TypeParser<T> TypeParser;

        public TypeSpanParser(IBinaryFormatter formatter) {
            Debug.Assert(!typeof(T).IsGenericType);
            if (!formatter.TryGetParser(out TypeParser)) {
                Debug.Fail("Formatter should have decided if we contain this type");
                throw new ArgumentException($"Unable to locate {typeof(T)} formatter");
            }
        }

        public bool TrySerialize(ReadOnlySpan<T> value, Span<byte> destination, out int writtenBytes) {
            writtenBytes = -1;
            var originalLength = destination.Length;
            for (var i = 0; i < value.Length; ++i) {
                if (!TypeParser.TrySerialize(value[i], destination, out var currentWrittenBytes))
                    return false;
                destination = destination.Slice(currentWrittenBytes);
            }
            writtenBytes = originalLength - destination.Length;
            return true;
        }

        public int GetBinaryLength(ReadOnlySpan<T> value) {
            // Optimize for types that have fixed size (e.g. ReadOnlySpan<int>)
            if (TypeParser.FixedSize.HasValue)
                return value.Length * TypeParser.FixedSize.Value;

            var length = 0;
            for (var i = 0; i < value.Length; ++i)
                length += TypeParser.GetBinaryLength(value[i]);
            return length;
        }

        public DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, Span<T> destination, out int readBytes) {
            readBytes = -1;
            var originalLength = span.Length;
            for (var i = 0; i < destination.Length; ++i) {
                var parseResult = TypeParser.TryDeserialize(span, out var currentReadBytes, out T result);
                if (parseResult != DeserializeResult.Success)
                    return parseResult;
                destination[i] = result;
                span = span.Slice(currentReadBytes);
            }
            readBytes = originalLength - span.Length;
            return DeserializeResult.Success;
        }
    }
}
