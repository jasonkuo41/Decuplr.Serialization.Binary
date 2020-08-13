using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Decuplr.Serialization.Binary.Internal;

namespace Decuplr.Serialization.Binary.Parsers {
    public static partial class PrimitiveParsers {

        #region Int16 (short)

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16(in ReadOnlySequence<byte> sequence, bool isSmallEndian) {
            // We predict that sequence would always provide enough space for us to read
#if NETSTANDARD2_0
            var memory = sequence.First;
            if (memory.Length >= sizeof(short))
                return ReadInt16Unsafe(memory.Span, isSmallEndian);
#else
            
            var span = sequence.FirstSpan;
            if (span.Length >= sizeof(short))
                return ReadInt16Unsafe(span, isSmallEndian);
#endif

            // Otherwise it's we just go with the slow and sluggish route
            // This is a copy of SequenceReader (almost)
            Span<byte> data = stackalloc byte[sizeof(short)];
            sequence.TrySlowCopyTo(data);
            return ReadInt16(data, isSmallEndian);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16(ReadOnlySpan<byte> span, bool isSmallEndian)
            => isSmallEndian ? BinaryPrimitives.ReadInt16LittleEndian(span) : BinaryPrimitives.ReadInt16BigEndian(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16Unsafe(ReadOnlySpan<byte> span, bool isSmallEndian) {
            var value = Unsafe.ReadUnaligned<short>(ref MemoryMarshal.GetReference(span));
            if (isSmallEndian != BitConverter.IsLittleEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReadInt16(ReadOnlySpan<byte> span, Span<short> destination, bool isSmallEndian) {
            if (isSmallEndian == BitConverter.IsLittleEndian) {
                span.CopyTo(MemoryMarshal.AsBytes(destination));
                return;
            }
            for (var i = 0; i < span.Length / sizeof(short); ++i) {
                destination[i] = ReadInt16Unsafe(span, isSmallEndian);
                span = span.Slice(sizeof(short));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt16(short value, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian)
                BinaryPrimitives.WriteInt16LittleEndian(destination, value);
            else
                BinaryPrimitives.WriteInt16BigEndian(destination, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt16Unsafe(short value, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian != BitConverter.IsLittleEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt16(ReadOnlySpan<short> values, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian == BitConverter.IsLittleEndian) {
                MemoryMarshal.AsBytes(values).CopyTo(destination);
                return;
            }
            for (var i = 0; i < destination.Length / sizeof(short); ++i) {
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), values[i]);
                destination = destination.Slice(sizeof(short));
            }
        }
        #endregion

        #region UInt16 (ushort)

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16(in ReadOnlySequence<byte> sequence, bool isSmallEndian) {
            // We predict that sequence would always provide enough space for us to read
#if NETSTANDARD2_0
            var memory = sequence.First;
            if (memory.Length >= sizeof(ushort))
                return ReadUInt16Unsafe(memory.Span, isSmallEndian);
#else
            
            var span = sequence.FirstSpan;
            if (span.Length >= sizeof(ushort))
                return ReadUInt16Unsafe(span, isSmallEndian);
#endif

            // Otherwise it's we just go with the slow and sluggish route
            // This is a copy of SequenceReader (almost)
            Span<byte> data = stackalloc byte[sizeof(ushort)];
            sequence.TrySlowCopyTo(data);
            return ReadUInt16(data, isSmallEndian);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16(ReadOnlySpan<byte> span, bool isSmallEndian)
            => isSmallEndian ? BinaryPrimitives.ReadUInt16LittleEndian(span) : BinaryPrimitives.ReadUInt16BigEndian(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16Unsafe(ReadOnlySpan<byte> span, bool isSmallEndian) {
            var value = Unsafe.ReadUnaligned<ushort>(ref MemoryMarshal.GetReference(span));
            if (isSmallEndian != BitConverter.IsLittleEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReadUInt16(ReadOnlySpan<byte> span, Span<ushort> destination, bool isSmallEndian) {
            if (isSmallEndian == BitConverter.IsLittleEndian) {
                span.CopyTo(MemoryMarshal.AsBytes(destination));
                return;
            }
            for (var i = 0; i < span.Length / sizeof(ushort); ++i) {
                destination[i] = ReadUInt16Unsafe(span, isSmallEndian);
                span = span.Slice(sizeof(ushort));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt16(ushort value, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian)
                BinaryPrimitives.WriteUInt16LittleEndian(destination, value);
            else
                BinaryPrimitives.WriteUInt16BigEndian(destination, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt16Unsafe(ushort value, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian != BitConverter.IsLittleEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt16(ReadOnlySpan<ushort> values, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian == BitConverter.IsLittleEndian) {
                MemoryMarshal.AsBytes(values).CopyTo(destination);
                return;
            }
            for (var i = 0; i < destination.Length / sizeof(ushort); ++i) {
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), values[i]);
                destination = destination.Slice(sizeof(ushort));
            }
        }
        #endregion

        #region UInt32 (uint)

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32(in ReadOnlySequence<byte> sequence, bool isSmallEndian) {
            // We predict that sequence would always provide enough space for us to read
#if NETSTANDARD2_0
            var memory = sequence.First;
            if (memory.Length >= sizeof(uint))
                return ReadUInt32Unsafe(memory.Span, isSmallEndian);
#else
            
            var span = sequence.FirstSpan;
            if (span.Length >= sizeof(uint))
                return ReadUInt32Unsafe(span, isSmallEndian);
#endif

            // Otherwise it's we just go with the slow and sluggish route
            // This is a copy of SequenceReader (almost)
            Span<byte> data = stackalloc byte[sizeof(uint)];
            sequence.TrySlowCopyTo(data);
            return ReadUInt32(data, isSmallEndian);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32(ReadOnlySpan<byte> span, bool isSmallEndian)
            => isSmallEndian ? BinaryPrimitives.ReadUInt32LittleEndian(span) : BinaryPrimitives.ReadUInt32BigEndian(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32Unsafe(ReadOnlySpan<byte> span, bool isSmallEndian) {
            var value = Unsafe.ReadUnaligned<uint>(ref MemoryMarshal.GetReference(span));
            if (isSmallEndian != BitConverter.IsLittleEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReadUInt32(ReadOnlySpan<byte> span, Span<uint> destination, bool isSmallEndian) {
            if (isSmallEndian == BitConverter.IsLittleEndian) {
                span.CopyTo(MemoryMarshal.AsBytes(destination));
                return;
            }
            for (var i = 0; i < span.Length / sizeof(uint); ++i) {
                destination[i] = ReadUInt32Unsafe(span, isSmallEndian);
                span = span.Slice(sizeof(uint));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt32(uint value, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian)
                BinaryPrimitives.WriteUInt32LittleEndian(destination, value);
            else
                BinaryPrimitives.WriteUInt32BigEndian(destination, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt32Unsafe(uint value, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian != BitConverter.IsLittleEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt32(ReadOnlySpan<uint> values, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian == BitConverter.IsLittleEndian) {
                MemoryMarshal.AsBytes(values).CopyTo(destination);
                return;
            }
            for (var i = 0; i < destination.Length / sizeof(uint); ++i) {
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), values[i]);
                destination = destination.Slice(sizeof(uint));
            }
        }
        #endregion

        #region Int64 (long)

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadInt64(in ReadOnlySequence<byte> sequence, bool isSmallEndian) {
            // We predict that sequence would always provide enough space for us to read
#if NETSTANDARD2_0
            var memory = sequence.First;
            if (memory.Length >= sizeof(long))
                return ReadInt64Unsafe(memory.Span, isSmallEndian);
#else
            
            var span = sequence.FirstSpan;
            if (span.Length >= sizeof(long))
                return ReadInt64Unsafe(span, isSmallEndian);
#endif

            // Otherwise it's we just go with the slow and sluggish route
            // This is a copy of SequenceReader (almost)
            Span<byte> data = stackalloc byte[sizeof(long)];
            sequence.TrySlowCopyTo(data);
            return ReadInt64(data, isSmallEndian);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadInt64(ReadOnlySpan<byte> span, bool isSmallEndian)
            => isSmallEndian ? BinaryPrimitives.ReadInt64LittleEndian(span) : BinaryPrimitives.ReadInt64BigEndian(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadInt64Unsafe(ReadOnlySpan<byte> span, bool isSmallEndian) {
            var value = Unsafe.ReadUnaligned<long>(ref MemoryMarshal.GetReference(span));
            if (isSmallEndian != BitConverter.IsLittleEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReadInt64(ReadOnlySpan<byte> span, Span<long> destination, bool isSmallEndian) {
            if (isSmallEndian == BitConverter.IsLittleEndian) {
                span.CopyTo(MemoryMarshal.AsBytes(destination));
                return;
            }
            for (var i = 0; i < span.Length / sizeof(long); ++i) {
                destination[i] = ReadInt64Unsafe(span, isSmallEndian);
                span = span.Slice(sizeof(long));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt64(long value, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian)
                BinaryPrimitives.WriteInt64LittleEndian(destination, value);
            else
                BinaryPrimitives.WriteInt64BigEndian(destination, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt64Unsafe(long value, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian != BitConverter.IsLittleEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt64(ReadOnlySpan<long> values, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian == BitConverter.IsLittleEndian) {
                MemoryMarshal.AsBytes(values).CopyTo(destination);
                return;
            }
            for (var i = 0; i < destination.Length / sizeof(long); ++i) {
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), values[i]);
                destination = destination.Slice(sizeof(long));
            }
        }
        #endregion

        #region UInt64 (long)

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUInt64(in ReadOnlySequence<byte> sequence, bool isSmallEndian) {
            // We predict that sequence would always provide enough space for us to read
#if NETSTANDARD2_0
            var memory = sequence.First;
            if (memory.Length >= sizeof(ulong))
                return ReadUInt64Unsafe(memory.Span, isSmallEndian);
#else
            
            var span = sequence.FirstSpan;
            if (span.Length >= sizeof(ulong))
                return ReadUInt64Unsafe(span, isSmallEndian);
#endif

            // Otherwise it's we just go with the slow and sluggish route
            // This is a copy of SequenceReader (almost)
            Span<byte> data = stackalloc byte[sizeof(ulong)];
            sequence.TrySlowCopyTo(data);
            return ReadUInt64(data, isSmallEndian);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUInt64(ReadOnlySpan<byte> span, bool isSmallEndian)
            => isSmallEndian ? BinaryPrimitives.ReadUInt64LittleEndian(span) : BinaryPrimitives.ReadUInt64BigEndian(span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUInt64Unsafe(ReadOnlySpan<byte> span, bool isSmallEndian) {
            var value = Unsafe.ReadUnaligned<ulong>(ref MemoryMarshal.GetReference(span));
            if (isSmallEndian != BitConverter.IsLittleEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReadUInt64(ReadOnlySpan<byte> span, Span<ulong> destination, bool isSmallEndian) {
            if (isSmallEndian == BitConverter.IsLittleEndian) {
                span.CopyTo(MemoryMarshal.AsBytes(destination));
                return;
            }
            for (var i = 0; i < span.Length / sizeof(ulong); ++i) {
                destination[i] = ReadUInt64Unsafe(span, isSmallEndian);
                span = span.Slice(sizeof(ulong));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt64(ulong value, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian)
                BinaryPrimitives.WriteUInt64LittleEndian(destination, value);
            else
                BinaryPrimitives.WriteUInt64BigEndian(destination, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt64Unsafe(ulong value, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian != BitConverter.IsLittleEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt64(ReadOnlySpan<ulong> values, Span<byte> destination, bool isSmallEndian) {
            if (isSmallEndian == BitConverter.IsLittleEndian) {
                MemoryMarshal.AsBytes(values).CopyTo(destination);
                return;
            }
            for (var i = 0; i < destination.Length / sizeof(ulong); ++i) {
                Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), values[i]);
                destination = destination.Slice(sizeof(ulong));
            }
        }

        #endregion
    }
}
