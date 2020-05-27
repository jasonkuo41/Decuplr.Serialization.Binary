using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decuplr.Serialization.Binary.Internal.DefaultParsers {
    // Uses Microsoft ToBinary and FromBinary

    [ParserNamespace("Default")]
    class DateTimeParser : UnmanagedParserBase<DateTime> {
        protected override DateTime GetValue(ReadOnlySpan<byte> value) {
            return DateTime.FromBinary(BinaryConverter.ToInt64(value));
        }

        protected override void WriteBytes(Span<byte> destination, DateTime value) {
            var bin = value.ToBinary();
            var result = BinaryConverter.TryWriteBytes(destination, bin);
            Debug.Assert(result);
        }
    }

    [ParserNamespace("Default")]
    class StringParser : TypeParser<string> {

    }
}
