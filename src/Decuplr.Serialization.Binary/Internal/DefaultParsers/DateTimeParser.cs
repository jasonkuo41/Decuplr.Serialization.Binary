using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decuplr.Serialization.Binary.Internal.DefaultParsers {
    // Uses Microsoft ToBinary and FromBinary

    [ParserNamespace("Default")]
    class DateTimeParser : UnmanagedParserBase<DateTime> {

        private readonly TypeParser<long> Packer;

        public DateTimeParser(IBinaryPacker packer) {
            Packer = packer.GetParser<long>();
        }

        protected override DateTime GetValue(ReadOnlySpan<byte> value) {
            var parseResult = Packer.TryDeserialize(value, out var readBytes, out var datetime);
            Debug.Assert(parseResult == DeserializeResult.Success && readBytes == FixedSize.Value);
            return DateTime.FromBinary(datetime);
        }

        protected override void WriteBytes(Span<byte> destination, DateTime value) {
            var bin = value.ToBinary();
            var result = Packer.TrySerialize(bin, destination, out var writtenBytes);
            Debug.Assert(result && writtenBytes == FixedSize.Value);
        }
    }

    [ParserNamespace("Default")]
    class StringParser : TypeParser<string> {
        public override int GetBinaryLength(string value) {
            throw new NotImplementedException();
        }

        public override DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out string result) {
            throw new NotImplementedException();
        }

        public override bool TrySerialize(string value, Span<byte> destination, out int writtenBytes) {
            throw new NotImplementedException();
        }
    }
}
