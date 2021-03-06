﻿using System;

namespace Decuplr.Serialization.Binary {
    /// <summary>
    /// Describes a provider that writes, read length of a collection from a binary array
    /// </summary>
    public interface ILengthProvider {

        int ReadLength(ReadOnlySpan<byte> data, out int remainderOffset, out int remainderLength);

        int GetWriteBinaryLength(ReadOnlySpan<byte> data, out ReadOnlySpan<byte> result);

        int WriteLength(int collectionLength, ReadOnlySpan<byte> data, Span<byte> destination);
    }

    /// <summary>
    /// Describes a provider that writes, read length of a collection but also mutates the data in process
    /// </summary>
    /// <remarks>
    /// This provider is commonly found for delimitters which may escape delimitters in between the data thus causing a mutation of the data.
    /// This is slow and should only be used for legacy reasons.
    /// </remarks>
    public interface ILengthMutateProvider {

        int ReadLength(ReadOnlySpan<byte> data, Span<byte> result, out int writtenBytes);

        int GetWriteBinaryLength(ReadOnlySpan<byte> data, out ReadOnlySpan<byte> result); // This name is pretty lame imo

        int WriteLength(int collectionLength, ReadOnlySpan<byte> data, Span<byte> destination);
    }

    public abstract class LengthProvider {
        /// <summary>
        /// Get's the length for this collection
        /// </summary>
        /// <param name="data"></param>
        /// <returns>-1 if unable to locate the length, -2 if this is not a collection</returns>
        public abstract int GetCollectionLength(ReadOnlySpan<byte> data);

        public abstract int WriteCollectionLength(int length, ReadOnlySpan<byte> data, Span<byte> destination);
    }

    /// <summary>
    /// Describes a provider that writes, read length of a collection but also mutates the serialized data in process
    /// </summary>
    /// <remarks>
    /// This provider is commonly found for delimitters which may escape delimitters in between the data thus causing a mutation of the data.
    /// This is slow and should only be used for legacy reasons.
    /// </remarks>
    public abstract class TransformableLengthProvider : LengthProvider {

        public abstract int GetCollectionLength(ReadOnlySpan<byte> data, Span<byte> result, out int writtenBytes);

        public abstract int GetWrittenBinarySize(int length, ReadOnlySpan<byte> serializedCollection);
    }

    /// <summary>
    /// Describes a provider that writes, read length of a collection from a binary array by appending data
    /// </summary>
    public abstract class AppendingLengthProvider : LengthProvider {

        public abstract int GetCollectionLength(ReadOnlySpan<byte> data, out int remainderOffset, out int remainderLength);

        public abstract int GetWrittenBinarySize(int length);
    }
}
