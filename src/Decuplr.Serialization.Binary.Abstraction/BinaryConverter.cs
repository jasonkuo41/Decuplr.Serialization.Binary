using System;
using System.Buffers;
using Decuplr.Serialization.Binary.Utility;

namespace Decuplr.Serialization.Binary {

    /// <summary>
    /// Provides the base class for serializing and deserializing <typeparamref name="T"/> objects into it's binary counterpart
    /// </summary>
    /// <typeparam name="T">The target type for serialization and deserialization</typeparam>
    public abstract class BinaryConverter<T> {

        /// <summary>
        /// If the converting type has a fixed binary length. If so <see cref="GetSpanLength(in T)"/> and <see cref="GetBlockLength(ReadOnlySpan{byte})"/> family should return the same value.
        /// </summary>
        public virtual int? FixedLength => null;

        /// <summary>
        /// Serialize the <paramref name="item"/> to it's binary form and write to a buffer via <paramref name="bufferWriter"/>.
        /// </summary>
        /// <param name="item">The item being serialized</param>
        /// <param name="bufferWriter">The buffer writer</param>
        public void Serialize(in T item, IBufferWriter<byte> bufferWriter) {
            var structBuffer = new StructBufferWriterWrap<byte>(bufferWriter ?? throw new ArgumentNullException(nameof(bufferWriter)));
            Serialize(item, ref structBuffer);
        }

        /// <summary>
        /// Serialize the <paramref name="item"/> to it's binary form and write to a buffer via <paramref name="bufferWriter"/> (struct based).
        /// </summary>
        /// <remarks>
        /// This method provides a supposedly faster way of serialization compared to <see cref="Serialize(in T, IBufferWriter{byte})"/>.
        /// </remarks>
        /// <typeparam name="TWriter">The struct based <see cref="IBufferWriter{T}"> type.</typeparam>
        /// <param name="item">The item being serialized.</param>
        /// <param name="bufferWriter">The struct based buffer writer.</param>
        public void Serialize<TWriter>(in T item, ref TWriter bufferWriter) where TWriter : struct, IBufferWriter<byte>
            => Serialize(item, new NoopWriteState(), ref bufferWriter);

        /// <summary>
        /// Serialize the <paramref name="item"/> to it's binary from and write to a buffer via <paramref name="bufferWriter"/> (struct based). 
        /// This method will also write every object that is being serialized to <paramref name="writeState"/> for observation.
        /// </summary>
        /// <typeparam name="TState">The struct based state object.</typeparam>
        /// <typeparam name="TWriter">The struct based <see cref="IBufferWriter{T}"> type.</typeparam>
        /// <param name="item">The item being serialized</param>
        /// <param name="writeState">The state object.</param>
        /// <param name="bufferWriter">The struct based buffer writer.</param>
        public virtual void Serialize<TState, TWriter>(in T item, TState writeState, ref TWriter bufferWriter)
            where TState : struct, IBinaryWriteState<TState>
            where TWriter : struct, IBufferWriter<byte> {
            var length = GetSpanLength(item);
            var span = bufferWriter.GetSpan(length);
            var writtenBytes = Serialize(item, span);
            bufferWriter.Advance(writtenBytes);
        }

        /// <summary>
        /// Get's the binary length of <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The object to retrieve it's binary form's length</param>
        /// <returns>The binary forms length</returns>
        public int GetSpanLength(in T item) => GetSpanLength(item, new NoopWriteState());

        /// <summary>
        /// Get's the binary length of <paramref name="item"/> with the consideration of a state object <paramref name="writeState"/> that recieves every object being serialized.
        /// </summary>
        /// <param name="item">The object to retrieve it's binary form's length</param>
        /// <returns>The binary forms length</returns>
        public abstract int GetSpanLength<TState>(in T item, TState writeState) where TState : struct, IBinaryWriteState<TState>;

        /// <summary>
        /// Serialize the <paramref name="item"/> to it's binary form and write to the span '<paramref name="data"/>'.
        /// </summary>
        /// <param name="item">The item being serialized.</param>
        /// <param name="data">The span that will receive the data.</param>
        /// <returns>How many bytes were written to the span, returns -1 if there are insufficent data.</returns>
        public int Serialize(in T item, Span<byte> data) => Serialize(item, new NoopWriteState(), data);

        public abstract int Serialize<TState>(in T item, TState writeState, Span<byte> data) where TState : struct, IBinaryWriteState<TState>;

        public abstract int Deserialize(ReadOnlySpan<byte> data, out T result);

        public SequenceReadResult<T> Deserialize(ReadOnlySequence<byte> sequence) {
            // Create cursor for this sequence
            var cursor = new SequenceCursor<byte>(in sequence);
            if (!Deserialize(ref cursor, out var result))
                return SequenceReadResult<T>.InsufficientData;
            return new SequenceReadResult<T>(result, cursor.Consumed, cursor.Position);
        }

        public abstract bool Deserialize(ref SequenceCursor<byte> cursor, out T result);

        public int GetBlockLength(ReadOnlySequence<byte> sequence) {
            var cursor = new SequenceCursor<byte>(in sequence);
            return GetBlockLength(ref cursor);
        }

        public abstract int GetBlockLength(ReadOnlySpan<byte> data);

        public abstract int GetBlockLength(ref SequenceCursor<byte> cursor);
    }

    /*
    *  We need to source gen 
    *  
    *  void Serialize<TState, TWriter>(in T item, in TState state, ref TWriter writer);
    *  int Serialize<TState>(in T item, in TState writeState, Span<byte> data);
    *  
    *  int GetSpanLength<TState>(in T item, in TState writeState);
    *  
    *  SpanReadResult<T> Deserialize(ReadOnlySpan<byte> data);
    *  T Deserialize(ref SequenceCursor<byte> cursor);
    *  
    *  int GetBlockLength(ReadOnlySpan<byte> data);
    *  int GetBlockLength(ref SequenceCursor<byte> cursor);
    *  
    */

    internal interface ISourceGenStub<TSource, TMember> {
        void Serialize<TState, TWriter>(in TMember member, in TSource item, TState state, ref TWriter writer);
        int Serialize<TState>(in TMember member, in TSource item, TState writeState, Span<byte> data);

        int GetSpanLength<TState>(in TMember member, in TSource item, TState writeState);

        SpanReadResult<TMember> Deserialize(ReadOnlySpan<byte> data);
        TMember Deserialize(ref SequenceCursor<byte> cursor);

        int GetBlockLength(ReadOnlySpan<byte> data);
        int GetBlockLength(ref SequenceCursor<byte> cursor);
    }
}
