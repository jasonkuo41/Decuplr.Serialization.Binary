using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the corefx project root for more information.

namespace Decuplr.Compatibility {

#if NETSTANDARD2_0

    // The class is meant for compatibility with .net standard 2.0 and .net standard 2.1
    // Direct modification of https://source.dot.net/#System.Private.CoreLib/shared/System/BitConverter.cs,8640d8adfffb155b
    // From Corefx
    internal static class BitConverterStd21 {

        // Converts a Boolean into a Span of bytes with length one.
        public static bool TryWriteBytes(Span<byte> destination, bool value) {
            if (destination.Length < sizeof(byte))
                return false;

            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value ? (byte)1 : (byte)0);
            return true;
        }

        // Converts a char into a Span
        public static bool TryWriteBytes(Span<byte> destination, char value) {
            if (destination.Length < sizeof(char))
                return false;

            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
            return true;
        }

        // Converts a short into a Span
        public static bool TryWriteBytes(Span<byte> destination, short value) {
            if (destination.Length < sizeof(short))
                return false;

            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
            return true;
        }

        // Converts an int into a Span
        public static bool TryWriteBytes(Span<byte> destination, int value) {
            if (destination.Length < sizeof(int))
                return false;

            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
            return true;
        }

        // Converts a long into a Span
        public static bool TryWriteBytes(Span<byte> destination, long value) {
            if (destination.Length < sizeof(long))
                return false;

            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
            return true;
        }

        // Converts a ushort into a Span
        public static bool TryWriteBytes(Span<byte> destination, ushort value) {
            if (destination.Length < sizeof(ushort))
                return false;

            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
            return true;
        }

        // Converts a uint into a Span
        public static bool TryWriteBytes(Span<byte> destination, uint value) {
            if (destination.Length < sizeof(uint))
                return false;

            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
            return true;
        }

        // Converts a ulong into a Span
        public static bool TryWriteBytes(Span<byte> destination, ulong value) {
            if (destination.Length < sizeof(ulong))
                return false;

            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
            return true;
        }

        // Converts a float into a Span
        public static bool TryWriteBytes(Span<byte> destination, float value) {
            if (destination.Length < sizeof(float))
                return false;

            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
            return true;
        }

        // Converts a double into a Span
        public static bool TryWriteBytes(Span<byte> destination, double value) {
            if (destination.Length < sizeof(double))
                return false;

            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
            return true;
        }

        // Converts a Span into a char
        public static char ToChar(ReadOnlySpan<byte> value) {
            if (value.Length < sizeof(char))
                throw new ArgumentOutOfRangeException(nameof(value));
            return Unsafe.ReadUnaligned<char>(ref MemoryMarshal.GetReference(value));
        }

        // Converts a Span into a short
        public static short ToInt16(ReadOnlySpan<byte> value) {
            if (value.Length < sizeof(short))
                throw new ArgumentOutOfRangeException(nameof(value));
            return Unsafe.ReadUnaligned<short>(ref MemoryMarshal.GetReference(value));
        }

        // Converts a Span into an int
        public static int ToInt32(ReadOnlySpan<byte> value) {
            if (value.Length < sizeof(int))
                throw new ArgumentOutOfRangeException(nameof(value));
            return Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(value));
        }

        // Converts a Span into a long
        public static long ToInt64(ReadOnlySpan<byte> value) {
            if (value.Length < sizeof(long))
                throw new ArgumentOutOfRangeException(nameof(value));
            return Unsafe.ReadUnaligned<long>(ref MemoryMarshal.GetReference(value));
        }

        // Converts a Span into a ushort
        public static ushort ToUInt16(ReadOnlySpan<byte> value) {
            if (value.Length < sizeof(ushort))
                throw new ArgumentOutOfRangeException(nameof(value));
            return Unsafe.ReadUnaligned<ushort>(ref MemoryMarshal.GetReference(value));
        }

        // Convert a Span into a uint
        public static uint ToUInt32(ReadOnlySpan<byte> value) {
            if (value.Length < sizeof(uint))
                throw new ArgumentOutOfRangeException(nameof(value));
            return Unsafe.ReadUnaligned<uint>(ref MemoryMarshal.GetReference(value));
        }

        // Converts a Span into an unsigned long
        public static ulong ToUInt64(ReadOnlySpan<byte> value) {
            if (value.Length < sizeof(ulong))
                throw new ArgumentOutOfRangeException(nameof(value));
            return Unsafe.ReadUnaligned<ulong>(ref MemoryMarshal.GetReference(value));
        }

        // Converts a Span into a float
        public static float ToSingle(ReadOnlySpan<byte> value) {
            if (value.Length < sizeof(float))
                throw new ArgumentOutOfRangeException(nameof(value));
            return Unsafe.ReadUnaligned<float>(ref MemoryMarshal.GetReference(value));
        }

        // Converts a Span into a double
        public static double ToDouble(ReadOnlySpan<byte> value) {
            if (value.Length < sizeof(double))
                throw new ArgumentOutOfRangeException(nameof(value));
            return Unsafe.ReadUnaligned<double>(ref MemoryMarshal.GetReference(value));
        }

        public static bool ToBoolean(ReadOnlySpan<byte> value) {
            if (value.Length < sizeof(byte))
                throw new ArgumentOutOfRangeException(nameof(value));
            return Unsafe.ReadUnaligned<byte>(ref MemoryMarshal.GetReference(value)) != 0;
        }
    }

#endif

}
