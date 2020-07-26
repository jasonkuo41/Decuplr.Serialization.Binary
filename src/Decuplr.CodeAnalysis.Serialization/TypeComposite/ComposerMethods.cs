using Decuplr.CodeAnalysis.Serialization.Arguments;
using Decuplr.CodeAnalysis.Serialization.TypeComposite.Internal;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite {
    public struct ComposerMethods {
        private readonly ComposerMethodNames _names;

        public ComposerMethods(ComposerMethodNames names) {
            _names = names;
        }

        public string TryDeserializeSpan(BufferArgs readOnlySpan, OutArgs<int> outReadBytes, OutArgs<object> outResult) 
            => $"{_names.TryDeserializeSpan}({readOnlySpan}, out {outReadBytes}, out {outResult})";

        public string TryDeserializeSequence(BufferArgs refSequenceCursor, OutArgs<object> outResult)
            => $"{_names.TryDeserializeSequence}(ref {refSequenceCursor}, out {outResult})";

        public string DeserializeSpan(BufferArgs readOnlySpan, OutArgs<int> outReadBytes)
            => $"{_names.DeserializeSpan}({readOnlySpan}, out {outReadBytes})";

        public string DeserializeSequence(BufferArgs refSequenceCursor)
            => $"{_names.DeserializeSequence}(ref {refSequenceCursor})";

        public string TrySerialize(InArgs<object> target, BufferArgs readOnlySpan, OutArgs<int> outWrittenBytes)
            => $"{_names.TrySerialize}({target}, {readOnlySpan}, out {outWrittenBytes})";

        public string Serialize(InArgs<object> target, BufferArgs readOnlySpan)
            => $"{_names.Serialize}({target}, {readOnlySpan})";

        public string GetLength(InArgs<object> target)
            => $"{_names.GetLength}({target})";
    }

    public struct ComposerMethods<TArgs> where TArgs : struct {
        private readonly ComposerMethodNames _names;

        public ComposerMethods(ComposerMethodNames names) {
            _names = names;
        }

        public string TryDeserializeSpan(TArgs input, BufferArgs readOnlySpan, OutArgs<int> outReadBytes, OutArgs<object> outResult)
            => $"{_names.TryDeserializeSpan}({input}, {readOnlySpan}, out {outReadBytes}, out {outResult})";

        public string TryDeserializeSequence(TArgs input, BufferArgs refSequenceCursor, OutArgs<object> outResult)
            => $"{_names.TryDeserializeSequence}({input}, ref {refSequenceCursor}, out {outResult})";

        public string DeserializeSpan(TArgs input, BufferArgs readOnlySpan, OutArgs<int> outReadBytes)
            => $"{_names.DeserializeSpan}({input}, {readOnlySpan}, out {outReadBytes})";

        public string DeserializeSequence(TArgs input, BufferArgs refSequenceCursor)
            => $"{_names.DeserializeSequence}({input}, ref {refSequenceCursor})";

        public string TrySerialize(TArgs input, InArgs<object> target, BufferArgs readOnlySpan, OutArgs<int> outWrittenBytes)
            => $"{_names.TrySerialize}({input}, {target}, {readOnlySpan}, out {outWrittenBytes})";

        public string Serialize(TArgs input, InArgs<object> target, BufferArgs readOnlySpan)
            => $"{_names.Serialize}({input}, {target}, {readOnlySpan})";

        public string GetLength(TArgs input, InArgs<object> target)
            => $"{_names.GetLength}({input}, {target})";
    }
}
