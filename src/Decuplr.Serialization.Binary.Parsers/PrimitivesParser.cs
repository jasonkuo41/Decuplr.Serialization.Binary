using System;
using System.Buffers.Binary;
using Decuplr.Serialization.Binary.Annotations;
using Decuplr.Serialization.Binary.Annotations.Namespaces;
using Decuplr.Serialization.Binary.Internal;

namespace Decuplr.Serialization.Binary.Parsers {
    /*
     * Generated using the following code
     * 
     * 
        [ParserNamespace("Default")]
        string GenerateParser<T>() {
            var ident = typeof(T).Name;
            return @$"
                class {ident}Parser : PrimitiveParserBase<{ident}> {{
                    protected override {ident} GetValue(ReadOnlySpan<byte> value) => BinaryPrimitives.Read{ident}LittleEndian(value);

                    protected override void WriteBytes(Span<byte> destination, {ident} value) => BinaryPrimitives.Write{ident}LittleEndian(destination, value);
                }}";
        }
      
        WriteLine(GenerateParser<Boolean>());
        WriteLine(GenerateParser<Byte>());
        WriteLine(GenerateParser<SByte>());
        WriteLine(GenerateParser<UInt16>());
        WriteLine(GenerateParser<UInt32>());
        WriteLine(GenerateParser<UInt64>());
        WriteLine(GenerateParser<Int16>());
        WriteLine(GenerateParser<Int32>());
        WriteLine(GenerateParser<Int64>());
        WriteLine(GenerateParser<Single>());
        WriteLine(GenerateParser<Double>());
        WriteLine(GenerateParser<Char>()); 
     *
     *  // Note we need to modify ByteParser and SByteParser
     */

    [BinaryParser]
    [BinaryParserNamespace("Default")]
    internal class BooleanParser : UnmanagedParserBase<bool> {
        protected override bool GetValue(ReadOnlySpan<byte> value) => value[0] > 0;
        protected override void WriteBytes(Span<byte> destination, bool value) => destination[0] = value ? (byte)1 : (byte)0;
    }

    [BinaryParser]
    [BinaryParserNamespace("Default")]
    internal class ByteParser : UnmanagedParserBase<byte> {
        protected override byte GetValue(ReadOnlySpan<byte> value) => value[0];
        protected override void WriteBytes(Span<byte> destination, byte value) => destination[0] = value;
    }

    [BinaryParser]
    [BinaryParserNamespace("Default")]
    internal class SByteParser : UnmanagedParserBase<sbyte> {
        protected override sbyte GetValue(ReadOnlySpan<byte> value) => (sbyte)value[0];
        protected override void WriteBytes(Span<byte> destination, sbyte value) => destination[0] = (byte)value;
    }

    [BinaryParser]
    [BinaryParserNamespace("Default")]
    internal class UInt16Parser : UnmanagedParserBase<ushort> {
        protected override ushort GetValue(ReadOnlySpan<byte> value) => BinaryPrimitives.ReadUInt16LittleEndian(value);
        protected override void WriteBytes(Span<byte> destination, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(destination, value);
    }

    [BinaryParser]
    [BinaryParserNamespace("Default")]
    internal class UInt32Parser : UnmanagedParserBase<uint> {
        protected override uint GetValue(ReadOnlySpan<byte> value) => BinaryPrimitives.ReadUInt32LittleEndian(value);
        protected override void WriteBytes(Span<byte> destination, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(destination, value);
    }

    [BinaryParser]
    [BinaryParserNamespace("Default")]
    internal class UInt64Parser : UnmanagedParserBase<ulong> {
        protected override ulong GetValue(ReadOnlySpan<byte> value) => BinaryPrimitives.ReadUInt64LittleEndian(value);
        protected override void WriteBytes(Span<byte> destination, ulong value) => BinaryPrimitives.WriteUInt64LittleEndian(destination, value);
    }

    [BinaryParser]
    [BinaryParserNamespace("Default")]
    internal class Int16Parser : UnmanagedParserBase<short> {
        protected override short GetValue(ReadOnlySpan<byte> value) => BinaryPrimitives.ReadInt16LittleEndian(value);
        protected override void WriteBytes(Span<byte> destination, short value) => BinaryPrimitives.WriteInt16LittleEndian(destination, value);
    }

    [BinaryParser]
    [BinaryParserNamespace("Default")]
    internal class Int32Parser : UnmanagedParserBase<int> {
        protected override int GetValue(ReadOnlySpan<byte> value) => BinaryPrimitives.ReadInt32LittleEndian(value);
        protected override void WriteBytes(Span<byte> destination, int value) => BinaryPrimitives.WriteInt32LittleEndian(destination, value);
    }

    internal class Int64Parser : UnmanagedParserBase<long> {
        protected override long GetValue(ReadOnlySpan<byte> value) => BinaryPrimitives.ReadInt64LittleEndian(value);
        protected override void WriteBytes(Span<byte> destination, long value) => BinaryPrimitives.WriteInt64LittleEndian(destination, value);
    }

    [BinaryParser]
    [BinaryParserNamespace("Default")]
    internal sealed class Int64ParserImproved : TypeParser<long> {

        public override int? FixedSize => sizeof(long);

        public override int GetBinaryLength(long value) => sizeof(long);

        public override int Serialize(long value, Span<byte> destination) {
            BinaryPrimitives.WriteInt64LittleEndian(destination, value);
            return sizeof(long);
        }

        public override DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out long result) {
            result = default;
            readBytes = -1;
            if (span.Length < sizeof(long))
                return DeserializeResult.InsufficientSize;
            result = BinaryPrimitives.ReadInt64LittleEndian(span);
            readBytes = sizeof(long);
            return DeserializeResult.Success;
        }

        public override bool TrySerialize(long value, Span<byte> destination, out int writtenBytes) {
            writtenBytes = -1;
            if (destination.Length < sizeof(long))
                return false;
            writtenBytes = Serialize(value, destination);
            return true;
        }
    }

    [BinaryParser]
    [BinaryParserNamespace("Default")]
    internal class SingleParser : UnmanagedParserBase<float> {
        protected override float GetValue(ReadOnlySpan<byte> value) => BinaryPrimitivesEx.ReadSingleLittleEndian(value);
        protected override void WriteBytes(Span<byte> destination, float value) => BinaryPrimitivesEx.WriteSingleLittleEndian(destination, value);
    }

    [BinaryParser]
    [BinaryParserNamespace("Default")]
    internal class DoubleParser : UnmanagedParserBase<double> {
        protected override double GetValue(ReadOnlySpan<byte> value) => BinaryPrimitivesEx.ReadDoubleLittleEndian(value);
        protected override void WriteBytes(Span<byte> destination, double value) => BinaryPrimitivesEx.WriteDoubleLittleEndian(destination, value);
    }

    [BinaryParser]
    [BinaryParserNamespace("Default")]
    internal class CharParser : UnmanagedParserBase<char> {
        protected override char GetValue(ReadOnlySpan<byte> value) => (char)BinaryPrimitives.ReadUInt16LittleEndian(value);
        protected override void WriteBytes(Span<byte> destination, char value) => BinaryPrimitives.WriteUInt16LittleEndian(destination, value);
    }

}
