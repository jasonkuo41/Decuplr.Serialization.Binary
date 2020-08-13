using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Decuplr.Serialization.Binary.Parsers {
    public static partial class PrimitiveParsers {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadChar(in ReadOnlySequence<byte> sequence, bool isSmallEndian) => (char)ReadUInt16(in sequence, isSmallEndian);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadChar(ReadOnlySpan<byte> span, bool isSmallEndian) => (char)ReadUInt16(span, isSmallEndian);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadCharUnsafe(ReadOnlySpan<byte> span, bool isSmallEndian) => (char)ReadUInt16Unsafe(span, isSmallEndian);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReadChar(ReadOnlySpan<byte> span, Span<char> destination, bool isSmallEndian) {
            if (isSmallEndian == BitConverter.IsLittleEndian) {
                span.CopyTo(MemoryMarshal.AsBytes(destination));
                return;
            }
            for (var i = 0; i < span.Length / sizeof(char); ++i) {
                destination[i] = ReadCharUnsafe(span, isSmallEndian);
                span = span.Slice(sizeof(char));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteChar(char value, Span<byte> destination, bool isSmallEndian) => WriteUInt16(value, destination, isSmallEndian);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteCharUnsafe(char value, Span<byte> destination, bool isSmallEndian) => WriteUInt16Unsafe(value, destination, isSmallEndian);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteChar(ReadOnlySpan<char> values, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian == BitConverter.IsLittleEndian) {
                MemoryMarshal.AsBytes(values).CopyTo(destination);
                return;
            }
            for (var i = 0; i < destination.Length / sizeof(char); ++i) {
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), values[i]);
                destination = destination.Slice(sizeof(char));
            }
        }


    }
}
