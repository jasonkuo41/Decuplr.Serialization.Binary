using System;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.Templates.ParserSource {
    // For primitives that are only one-byte wide (bool, byte, sbyte)
    internal class PrimitiveByteDependency : ParserDependencyPrecusor {

        private readonly ITypeSymbol TypeSymbol;

        public PrimitiveByteDependency(ITypeSymbol typeSymbol) {
            TypeSymbol = typeSymbol;
        }

        private string ThrowArgOutOfRange(string source) => $"throw new {nameof(ArgumentOutOfRangeException)}(nameof({source}))";

        protected override string TrySerializeUnsafeSpan(ParsingTypeArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination, OutArgs<int> writtenBytes) {
            return $"{destination}[0] = (byte){fieldName}; return true;";
        }

        protected override string SerializeUnsafeSpan(ParsingTypeArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination) {
            return $"{destination}[0] = (byte){fieldName}; return true;";
        }

        protected override string TryDeserializeSpan(ParsingTypeArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes, OutArgs<object> result) {
            var node = new CodeNodeBuilder();
            node.AddStatement($"if ({source}.Length < 1) return {DeserializeResult.InsufficientSize.ToDisplayString()}");
            node.AddStatement($"{result} = ({TypeSymbol}){source}[0]");
            node.AddStatement($"{readBytes} = 1");
            node.AddStatement($"return {DeserializeResult.Success.ToDisplayString()}");
            return node.ToString();
        }

        protected override string TryDeserializeSequence(ParsingTypeArgs collectionArgs, BufferArgs source, OutArgs<object> result) {
            var node = new CodeNodeBuilder();
            // Since cursor always garuantee some space left in the span, unless it's completed (point to end)
            node.AddStatement($"if ({source}.Completed) return {DeserializeResult.InsufficientSize.ToDisplayString()}");
            node.AddStatement($"{result} = ({TypeSymbol}){source}.UnreadSpan[0]");
            node.AddStatement($"{source}.Advance(1)");
            node.AddStatement($"return {DeserializeResult.Success.ToDisplayString()}");
            return node.ToString();
        }

        protected override string DeserializeSpan(ParsingTypeArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes) {
            var node = new CodeNodeBuilder();
            node.AddStatement($"if ({source}.Length < 1) {ThrowArgOutOfRange(source.ToString())}");
            node.AddStatement($"{readBytes} = 1");
            node.AddStatement($"return ({TypeSymbol}){source}[0]");
            return node.ToString();
        }

        protected override string DeserializeSequence(ParsingTypeArgs collectionArgs, BufferArgs source) {
            var node = new CodeNodeBuilder();
            node.AddStatement($"if ({source}.IsCompleted) {ThrowArgOutOfRange(source.ToString())}");
            node.AddStatement($"var tempResult = ({TypeSymbol}){source}.UnreadSpan[0]");
            node.AddStatement($"{source}.Advance(1)");
            node.AddStatement($"return tempResult");
            return node.ToString();
        }

        protected override string GetLengthFunction(ParsingTypeArgs collectionArgs, TargetFieldArgs fieldName) => "return 1;";
    }
}
