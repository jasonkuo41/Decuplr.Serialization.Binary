using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Decuplr.Serialization.Binary.Internal;

/*  Replaced with the following code
 *  
		var regex = new Regex(@"(Int32)");
		Ex = regex.Replace(Ex, "Int64");
		regex = new Regex(@"(int)+");
		Console.WriteLine(regex.Replace(Ex, "long"));
 *
 */

namespace Decuplr.Serialization.Binary.Parsers {

    public static partial class PrimitiveParsers {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32(in ReadOnlySequence<byte> sequence, bool isSmallEndian) {
            // We predict that sequence would always provide enough space for us to read
#if NETSTANDARD2_0
            var memory = sequence.First;
            if (memory.Length >= sizeof(int))
                return ReadInt32Unsafe(memory.Span, isSmallEndian);
#else
            
            var span = sequence.FirstSpan;
            if (span.Length >= sizeof(int))
                return ReadInt32Unsafe(span, isSmallEndian);
#endif

            // Otherwise it's we just go with the slow and sluggish route
            // This is a copy of SequenceReader (almost)
            Span<byte> data = stackalloc byte[sizeof(int)];
            sequence.TrySlowCopyTo(data);
            return ReadInt32(data, isSmallEndian);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32(ReadOnlySpan<byte> span, bool isSmallEndian) 
            => isSmallEndian ? BinaryPrimitives.ReadInt32LittleEndian(span) : BinaryPrimitives.ReadInt32BigEndian(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32Unsafe(ReadOnlySpan<byte> span, bool isSmallEndian) {
            var value = Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(span));
            if (isSmallEndian != BitConverter.IsLittleEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReadInt32(ReadOnlySpan<byte> span, Span<int> destination, bool isSmallEndian) {
            if (isSmallEndian == BitConverter.IsLittleEndian) {
                span.CopyTo(MemoryMarshal.AsBytes(destination));
                return;
            }
            for (var i = 0; i < span.Length / sizeof(int); ++i) {
                destination[i] = ReadInt32Unsafe(span, isSmallEndian);
                span = span.Slice(sizeof(int));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt32(int value, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian)
                BinaryPrimitives.WriteInt32LittleEndian(destination, value);
            else
                BinaryPrimitives.WriteInt32BigEndian(destination, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt32Unsafe(int value, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian != BitConverter.IsLittleEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt32(ReadOnlySpan<int> values, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian == BitConverter.IsLittleEndian) {
                MemoryMarshal.AsBytes(values).CopyTo(destination);
                return;
            }
            for(var i = 0; i < destination.Length / sizeof(int); ++i) {
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), values[i]);
                destination = destination.Slice(sizeof(int));
            }
        }

    }

}
