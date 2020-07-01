namespace Decuplr.Serialization.Binary.CodeGenerator.Arguments {
    internal class TryDeserializeSpanArgs {
        public TryDeserializeSpanArgs(string typeSource, string readOnlySpan, string outReadBytes, string outResult) {
            TypeSource = typeSource;
            ReadOnlySpan = readOnlySpan;
            OutReadBytes = outReadBytes;
            OutResult = outResult;
        }

        public string TypeSource { get; }

        public string ReadOnlySpan { get; }

        public string OutReadBytes { get; }

        public string OutResult { get; }
    }

    internal class TryDeserializeSequenceArgs {
        public TryDeserializeSequenceArgs(string typeSource, string refSequenceCursor, string outResult) {
            TypeSource = typeSource;
            RefSequenceCursor = refSequenceCursor;
            OutResult = outResult;
        }

        public string TypeSource { get; }

        public string RefSequenceCursor { get; }

        public string OutResult { get; }
    }

    internal class DeserializeSpanArgs {
        public DeserializeSpanArgs(string typeSource, string readOnlySpan, string outResult) {
            TypeSource = typeSource;
            ReadOnlySpan = readOnlySpan;
            OutResult = outResult;
        }

        public string TypeSource { get; }

        public string ReadOnlySpan { get; }

        public string OutResult { get; }
    }

    internal class DeserializeSequenceArgs {
        public DeserializeSequenceArgs(string typeSource, string refSequenceCursor) {
            TypeSource = typeSource;
            RefSequenceCursor = refSequenceCursor;
        }

        public string TypeSource { get; }

        public string RefSequenceCursor { get; }
    }
}
