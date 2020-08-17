using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Decuplr.Serialization.Binary.Utility {
    /// <summary>
    /// Provides a struct wrapper around <see cref="IBufferWriter{T}"/>
    /// </summary>
    internal struct StructBufferWriterWrap<TData> : IBufferWriter<TData> {

        private readonly IBufferWriter<TData> _writer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StructBufferWriterWrap(IBufferWriter<TData> writer)
            => _writer = writer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count) {
            Debug.Assert(_writer != null);
            _writer.Advance(count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<TData> GetMemory(int sizeHint = 0) {
            Debug.Assert(_writer != null);
            return _writer.GetMemory(sizeHint);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<TData> GetSpan(int sizeHint = 0) {
            Debug.Assert(_writer != null);
            return _writer.GetSpan(sizeHint);
        }

    }

}
