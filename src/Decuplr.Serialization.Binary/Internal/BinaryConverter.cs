using System;

#if NETSTANDARD2_1
    using Converter = System.BitConverter;
#else
    using Converter = Decuplr.Compatibility.BitConverterStd21;
#endif

namespace Decuplr {
    /// <summary>
    /// A compatibility wrapper for <see cref="BitConverter"/> targeted both net standard 2.0 and 2.1
    /// </summary>
    public static class BinaryConverter {
        public static bool IsLittleEndian { get; } = BitConverter.IsLittleEndian;

        public static byte[] GetBytes(bool value) => BitConverter.GetBytes(value);
        public static bool TryWriteBytes(Span<byte> destination, bool value) => Converter.TryWriteBytes(destination, value);
        
        public static byte[] GetBytes(char value) => BitConverter.GetBytes(value);
        public static bool TryWriteBytes(Span<byte> destination, char value) => Converter.TryWriteBytes(destination, value);

        public static byte[] GetBytes(short value) => BitConverter.GetBytes(value);
        public static bool TryWriteBytes(Span<byte> destination, short value) => Converter.TryWriteBytes(destination, value);

        public static byte[] GetBytes(int value) => BitConverter.GetBytes(value);
        public static bool TryWriteBytes(Span<byte> destination, int value) => Converter.TryWriteBytes(destination, value);
    
        public static byte[] GetBytes(long value) => BitConverter.GetBytes(value);
        public static bool TryWriteBytes(Span<byte> destination, long value) => Converter.TryWriteBytes(destination, value);

        public static byte[] GetBytes(ushort value) => BitConverter.GetBytes(value);
        public static bool TryWriteBytes(Span<byte> destination, ushort value) => Converter.TryWriteBytes(destination, value);

        public static byte[] GetBytes(uint value) => BitConverter.GetBytes(value);
        public static bool TryWriteBytes(Span<byte> destination, uint value) => Converter.TryWriteBytes(destination, value);

        public static byte[] GetBytes(ulong value) => BitConverter.GetBytes(value);
        public static bool TryWriteBytes(Span<byte> destination, ulong value) => Converter.TryWriteBytes(destination, value);

        public static byte[] GetBytes(float value) => BitConverter.GetBytes(value);
        public static bool TryWriteBytes(Span<byte> destination, float value) => Converter.TryWriteBytes(destination, value);

        public static byte[] GetBytes(double value) => BitConverter.GetBytes(value);
        public static bool TryWriteBytes(Span<byte> destination, double value) => Converter.TryWriteBytes(destination, value);

        public static char ToChar(byte[] value, int startIndex) => BitConverter.ToChar(value, startIndex);
        public static char ToChar(ReadOnlySpan<byte> value) => Converter.ToChar(value);

        public static short ToInt16(byte[] value, int startIndex) => BitConverter.ToInt16(value, startIndex);
        public static short ToInt16(ReadOnlySpan<byte> value) => Converter.ToInt16(value);

        public static int ToInt32(byte[] value, int startIndex) => BitConverter.ToInt32(value, startIndex);
        public static int ToInt32(ReadOnlySpan<byte> value) => Converter.ToInt32(value);

        public static long ToInt64(byte[] value, int startIndex) => BitConverter.ToInt64(value, startIndex);
        public static long ToInt64(ReadOnlySpan<byte> value) => Converter.ToInt64(value);

        public static ushort ToUInt16(byte[] value, int startIndex) => BitConverter.ToUInt16(value, startIndex);
        public static ushort ToUInt16(ReadOnlySpan<byte> value) => Converter.ToUInt16(value);

        public static uint ToUInt32(byte[] value, int startIndex) => BitConverter.ToUInt32(value, startIndex);
        public static uint ToUInt32(ReadOnlySpan<byte> value) => Converter.ToUInt32(value);

        public static ulong ToUInt64(byte[] value, int startIndex) => BitConverter.ToUInt64(value, startIndex);
        public static ulong ToUInt64(ReadOnlySpan<byte> value) => Converter.ToUInt64(value);

        public static float ToSingle(byte[] value, int startIndex) => BitConverter.ToSingle(value, startIndex);
        public static float ToSingle(ReadOnlySpan<byte> value) => Converter.ToSingle(value);

        public static double ToDouble(byte[] value, int startIndex) => BitConverter.ToDouble(value, startIndex);
        public static double ToDouble(ReadOnlySpan<byte> value) => Converter.ToDouble(value);

        public static string ToString(byte[] value, int startIndex, int length) => BitConverter.ToString(value, startIndex, length);
        public static string ToString(byte[] value) => BitConverter.ToString(value);
        public static string ToString(byte[] value, int startIndex) => BitConverter.ToString(value, startIndex);

        public static bool ToBoolean(byte[] value, int startIndex) => BitConverter.ToBoolean(value, startIndex);
        public static bool ToBoolean(ReadOnlySpan<byte> value) => Converter.ToBoolean(value);
    }
}
