using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Decuplr.Serialization.Binary.Parsers {
    public static partial class PrimitiveParsers {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadDouble(in ReadOnlySequence<byte> sequence, bool isSmallEndian) {
            // We predict that sequence would always provide enough space for us to read
#if NETSTANDARD2_0
            var memory = sequence.First;
            if (memory.Length >= sizeof(double))
                return ReadDoubleUnsafe(memory.Span, isSmallEndian);
#else
            
            var span = sequence.FirstSpan;
            if (span.Length >= sizeof(double))
                return ReadDoubleUnsafe(span, isSmallEndian);
#endif

            // Otherwise it's we just go with the slow and sluggish route
            // This is a copy of SequenceReader (almost)
            Span<byte> data = stackalloc byte[sizeof(double)];
            sequence.TrySlowCopyTo(data);
            return ReadDouble(data, isSmallEndian);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadDouble(ReadOnlySpan<byte> span, bool isSmallEndian)
            => isSmallEndian ? BinaryPrimitivesEx.ReadDoubleLittleEndian(span) : BinaryPrimitivesEx.ReadDoubleBigEndian(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadDoubleUnsafe(ReadOnlySpan<byte> span, bool isSmallEndian) {
            if (isSmallEndian != BitConverter.IsLittleEndian) {
                var value = Unsafe.ReadUnaligned<long>(ref MemoryMarshal.GetReference(span));
                value = BinaryPrimitives.ReverseEndianness(value);
                return BitConverter.Int64BitsToDouble(value);
            }
            return Unsafe.ReadUnaligned<double>(ref MemoryMarshal.GetReference(span));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReadDouble(ReadOnlySpan<byte> span, Span<double> destination, bool isSmallEndian) {
            if (isSmallEndian == BitConverter.IsLittleEndian) {
                span.CopyTo(MemoryMarshal.AsBytes(destination));
                return;
            }
            for (var i = 0; i < span.Length / sizeof(double); ++i) {
                destination[i] = ReadDoubleUnsafe(span, isSmallEndian);
                span = span.Slice(sizeof(double));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteDouble(double value, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian)
                BinaryPrimitivesEx.WriteDoubleLittleEndian(destination, value);
            else
                BinaryPrimitivesEx.WriteDoubleBigEndian(destination, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteDoubleUnsafe(double value, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian != BitConverter.IsLittleEndian) {
                var temp = BinaryPrimitives.ReverseEndianness(BitConverter.DoubleToInt64Bits(value));
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), temp);
            }
            else {
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteDouble(ReadOnlySpan<double> values, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian == BitConverter.IsLittleEndian) {
                MemoryMarshal.AsBytes(values).CopyTo(destination);
                return;
            }
            for (var i = 0; i < destination.Length / sizeof(double); ++i) {
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), values[i]);
                destination = destination.Slice(sizeof(double));
            }
        }

    }
}
