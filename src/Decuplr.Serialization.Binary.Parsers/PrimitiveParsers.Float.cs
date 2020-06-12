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
        public static float ReadSingle(in ReadOnlySequence<byte> sequence, bool isSmallEndian) {
            // We predict that sequence would always provide enough space for us to read
#if NETSTANDARD2_0
            var memory = sequence.First;
            if (memory.Length >= sizeof(float))
                return ReadSingleUnsafe(memory.Span, isSmallEndian);
#else
            
            var span = sequence.FirstSpan;
            if (span.Length >= sizeof(float))
                return ReadSingleUnsafe(span, isSmallEndian);
#endif

            // Otherwise it's we just go with the slow and sluggish route
            // This is a copy of SequenceReader (almost)
            Span<byte> data = stackalloc byte[sizeof(float)];
            sequence.TrySlowCopyTo(data);
            return ReadSingle(data, isSmallEndian);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadSingle(ReadOnlySpan<byte> span, bool isSmallEndian)
            => isSmallEndian ? BinaryPrimitivesEx.ReadSingleLittleEndian(span) : BinaryPrimitivesEx.ReadSingleBigEndian(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadSingleUnsafe(ReadOnlySpan<byte> span, bool isSmallEndian) {
            if (isSmallEndian != BitConverter.IsLittleEndian) {
                var value = Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(span));
                return BinaryPrimitivesEx.Int32BitsToSingle(BinaryPrimitives.ReverseEndianness(value));
            }
            return Unsafe.ReadUnaligned<float>(ref MemoryMarshal.GetReference(span));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReadSingle(ReadOnlySpan<byte> span, Span<float> destination, bool isSmallEndian) {
            if (isSmallEndian == BitConverter.IsLittleEndian) {
                span.CopyTo(MemoryMarshal.AsBytes(destination));
                return;
            }
            for (var i = 0; i < span.Length / sizeof(float); ++i) {
                destination[i] = ReadSingleUnsafe(span, isSmallEndian);
                span = span.Slice(sizeof(float));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteSingle(float value, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian)
                BinaryPrimitivesEx.WriteSingleLittleEndian(destination, value);
            else
                BinaryPrimitivesEx.WriteSingleBigEndian(destination, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteSingleUnsafe(float value, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian != BitConverter.IsLittleEndian) {
                var temp = BinaryPrimitives.ReverseEndianness(BinaryPrimitivesEx.SingleToInt32Bits(value));
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), temp);
            }
            else {
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteSingle(ReadOnlySpan<float> values, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian == BitConverter.IsLittleEndian) {
                MemoryMarshal.AsBytes(values).CopyTo(destination);
                return;
            }
            for (var i = 0; i < destination.Length / sizeof(float); ++i) {
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), values[i]);
                destination = destination.Slice(sizeof(float));
            }
        }


    }
}
