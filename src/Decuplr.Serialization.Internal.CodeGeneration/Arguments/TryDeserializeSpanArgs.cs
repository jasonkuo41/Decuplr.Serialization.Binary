using System;

namespace Decuplr.Serialization.CodeGeneration.Arguments {
    public struct TryDeserializeSpanArgs<TSource> {

        public TSource Source { get; set; }

        public BufferArgs ReadOnlySpan { get; set; }

        public OutArgs<int> OutReadBytes { get; set; }

        public OutArgs<object> OutResult { get; set; }
    }

    public struct TryDeserializeSequenceArgs<TSource> {

        public TSource Source { get; set; }

        public BufferArgs RefSequenceCursor { get; set; }

        public OutArgs<object> OutResult { get; set; }
    }

    public struct DeserializeSpanArgs<TSource> {

        public TSource Source { get; set; }

        public BufferArgs ReadOnlySpan { get; set; }

        public OutArgs<object> OutResult { get; set; }
    }

    public struct DeserializeSequenceArgs<TSource> {

        public TSource Source { get; set; }

        public BufferArgs RefSequenceCursor { get; set; }
    }

    public struct SerializeArgs<TSource> {

        public TSource Source { get; set; }

        public BufferArgs ReadOnlySpan { get; set; }
    }

    public struct TrySerializeArgs<TSource> {

        public TSource Source { get; set; }

        public BufferArgs ReadOnlySpan { get; set; }

        public OutArgs<int> OutWrittenBytes { get; set; }
    }
}
