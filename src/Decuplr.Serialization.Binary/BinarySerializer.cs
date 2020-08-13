﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary {
    public static class BinarySerializer {
        public static byte[] Serialize<T>(in T item, BinarySerializerOptions? options = null) {
            using var buffer = new PooledByteBufferWriter(256);
            Serialize(item, buffer, options);
            return buffer.WrittenMemory.ToArray();
        }

        public static void Serialize<T>(in T item, IBufferWriter<byte> data, BinarySerializerOptions? options = null) {
            var length = GetSpanLength(item, options);
            var span = data.GetSpan(length);
            var writtenBytes = Serialize(item, span, ref length);
            data.Advance(writtenBytes);
        }

        public static int Serialize<T>(in T item, Span<byte> data, ref BinaryLength length) {

        }

        public static BinaryLength GetSpanLength<T>(in T item, BinarySerializerOptions? options = null) {
            options ??= BinarySerializerOptions.Default;

        }

        public static ReadResult<T> Deserialize<T>(ReadOnlySpan<byte> data, BinarySerializerOptions? options = null) {
            options ??= BinarySerializerOptions.Default;

        }

        public static ReadResult<T> Deserialize<T>(ReadOnlySequence<byte> data, BinarySerializerOptions? options = null) {
            options ??= BinarySerializerOptions.Default;

        }

        /// <summary>
        /// Inspects the binary data and determinates the binary length this type occupies without actually deserializing.
        /// Returns -1 if the data is insufficient to deteminate 
        /// </summary>
        /// <returns>The length of the binary blob that this </returns>
        public static int GetBlockLength<T>(ReadOnlySpan<byte> data, BinarySerializerOptions? options = null) {
            options ??= BinarySerializerOptions.Default;

        }

        public static int GetBlockLength<T>(ReadOnlySequence<byte> data, BinarySerializerOptions? options = null) {

        }
    }

}
