
#if USE_OBSOLETE

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Decuplr.Serialization.Binary.Internal {
    internal abstract class CollectionParserBase<T> : CollectionParser<T> {

        private readonly TypeParser<T> TypeParser;
        private readonly ICollectionLengthFormatter LengthFormatter;

        protected CollectionParserBase(IBinaryPacker formatter, INamespaceProvider binaryNamespace, ICollectionLengthFormatter lengthFormatter) 
            : base(formatter, binaryNamespace) {
            LengthFormatter = lengthFormatter;
            var result = formatter.TryGetParser(out TypeParser);
            // Assert that the result must be "true", the formatter should have known if this type is serializable
            Debug.Assert(result);
        }

        private bool TryAppendLengthCore(int length, Span<byte> destination, out Span<byte> leftOver) {
            leftOver = default;
            if (!LengthFormatter.TryWriteCollectionLength(length, destination, out var writtenBytes))
                return false;
            leftOver = destination.Slice(writtenBytes);
            return true;
        }

        private bool TryAppendLengthFront(int length, Span<byte> destination, out Span<byte> leftOver) {
            leftOver = destination;
            if (LengthFormatter.IsAppendFront)
                return true;
            return TryAppendLengthCore(length, destination, out leftOver);
        }

        private bool TryAppendLengthBack(int length, Span<byte> destination, out Span<byte> leftOver) {
            leftOver = destination;
            if (!LengthFormatter.IsAppendFront)
                return true;
            return TryAppendLengthCore(length, destination, out leftOver);
        }

        public override bool TrySerialize(ReadOnlySpan<T> value, Span<byte> destination, out int writtenBytes) {
            writtenBytes = -1;
            var originalLength = destination.Length;
            if (!TryAppendLengthFront(value.Length, destination, out destination))
                return false;

            for (var i = 0; i < value.Length; ++i) {
                if (!TypeParser.TrySerialize(value[i], destination, out var currentWrittenBytes))
                    return false;
                destination = destination.Slice(currentWrittenBytes);
            }

            if (!TryAppendLengthBack(value.Length, destination, out destination))
                return false;

            writtenBytes = originalLength - destination.Length;
            return true;
        }

        public override bool TrySerialize(IEnumerable<T> value, Span<byte> destination, out int writtenBytes) {
            writtenBytes = -1;
            var originalLength = destination.Length;
            if (!TryAppendLengthFront(value.Length, destination, out destination))
                return false;

            for (var i = 0; i < value.Length; ++i) {
                if (!TypeParser.TrySerialize(value[i], destination, out var currentWrittenBytes))
                    return false;
                destination = destination.Slice(currentWrittenBytes);
            }

            if (!TryAppendLengthBack(value.Length, destination, out destination))
                return false;

            writtenBytes = originalLength - destination.Length;
            return true;
        }

        // TODO : add support for HastSet ISet
        // TODO : add more System.Collection.Generics classes for support here
    }
}

#endif