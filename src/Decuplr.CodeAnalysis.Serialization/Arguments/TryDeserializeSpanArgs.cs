using System;

namespace Decuplr.CodeAnalysis.Serialization.Arguments {
    public struct TryDeserializeSpanArgs<TSource> {
        public TryDeserializeSpanArgs(TSource source, BufferArgs readOnlySpan, OutArgs<int> outReadBytes, OutArgs<object> outResult) {
            Source = source;
            ReadOnlySpan = readOnlySpan;
            OutReadBytes = outReadBytes;
            OutResult = outResult;
        }

        public TSource Source { get; set; }

        public BufferArgs ReadOnlySpan { get; set; }

        public OutArgs<int> OutReadBytes { get; set; }

        public OutArgs<object> OutResult { get; set; }
    }

    public struct TryDeserializeSequenceArgs<TSource> {

        public TryDeserializeSequenceArgs(TSource source, BufferArgs refSequenceCursor, OutArgs<object> outResult) {
            Source = source;
            RefSequenceCursor = refSequenceCursor;
            OutResult = outResult;
        }

        public TSource Source { get; set; }

        public BufferArgs RefSequenceCursor { get; set; }

        public OutArgs<object> OutResult { get; set; }
    }

    public struct DeserializeSpanArgs<TSource> {
        public DeserializeSpanArgs(TSource source, BufferArgs readOnlySpan, OutArgs<int> outReadBytes) {
            Source = source;
            ReadOnlySpan = readOnlySpan;
            OutReadBytes = outReadBytes;
        }

        public TSource Source { get; set; }

        public BufferArgs ReadOnlySpan { get; set; }

        public OutArgs<int> OutReadBytes { get; set; }
    }

    public struct DeserializeSequenceArgs<TSource> {
        public DeserializeSequenceArgs(TSource source, BufferArgs refSequenceCursor) {
            Source = source;
            RefSequenceCursor = refSequenceCursor;
        }

        public TSource Source { get; set; }

        public BufferArgs RefSequenceCursor { get; set; }
    }

    public struct SerializeArgs<TSource> {
        public SerializeArgs(TSource source, InArgs<object> target, BufferArgs readOnlySpan) {
            Source = source;
            Target = target;
            ReadOnlySpan = readOnlySpan;
        }

        public TSource Source { get; set; }

        public InArgs<object> Target { get; set; }

        public BufferArgs ReadOnlySpan { get; set; }
    }

    public struct TrySerializeArgs<TSource> {
        public TrySerializeArgs(TSource source, InArgs<object> target, BufferArgs readOnlySpan, OutArgs<int> outWrittenBytes) {
            Source = source;
            Target = target;
            ReadOnlySpan = readOnlySpan;
            OutWrittenBytes = outWrittenBytes;
        }

        public TSource Source { get; set; }

        public InArgs<object> Target { get; set; }

        public BufferArgs ReadOnlySpan { get; set; }

        public OutArgs<int> OutWrittenBytes { get; set; }
    }

    public struct GetLengthArgs<TSource> {
        public GetLengthArgs(TSource source, InArgs<object> target) {
            Source = source;
            Target = target;
        }

        public TSource Source { get; set; }

        public InArgs<object> Target { get; set; }

    }
}
