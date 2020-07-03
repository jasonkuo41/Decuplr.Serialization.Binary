using System;
using Decuplr.Serialization.Binary.Arguments;

namespace Decuplr.Serialization.Binary.CodeGenerator.Arguments {
    internal struct TryDeserializeSpanArgs<TSource> {

        public TSource Source { get; set; }

        public BufferArgs ReadOnlySpan { get; set; }

        public OutArgs<int> OutReadBytes { get; set; }

        public OutArgs<object> OutResult { get; set; }
    }

    internal struct TryDeserializeSequenceArgs<TSource> {

        public TSource Source { get; set; }

        public BufferArgs RefSequenceCursor { get; set; }

        public OutArgs<object> OutResult { get; set; }
    }

    internal struct DeserializeSpanArgs<TSource> {
        
        public TSource Source { get; set; }

        public BufferArgs ReadOnlySpan { get; set; }

        public OutArgs<object> OutResult { get; set; }
    }

    internal struct DeserializeSequenceArgs<TSource> {
        
        public TSource Source { get; set; }

        public BufferArgs RefSequenceCursor { get; set; }
    }

    internal struct SerializeArgs<TSource> {

        public TSource Source { get; set; }

        public BufferArgs ReadOnlySpan { get; set; }
    }

    internal struct TrySerializeArgs<TSource> {

        public TSource Source { get; set; }

        public BufferArgs ReadOnlySpan { get; set; }

        public OutArgs<int> OutWrittenBytes { get; set; }
    }
}
