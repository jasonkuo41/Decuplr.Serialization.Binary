using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.Templates.ParserSource {
    internal class TypeParserWrappedDependency : ParserDependencyPrecusor {

        private class WrappedTypeParser : IParserDependencyComponent {

            private readonly ITypeSymbol Symbol;

            public string TypeName { get; }

            public WrappedTypeParser(ITypeSymbol typeSymbol) {
                Symbol = typeSymbol;
                TypeName = $"TypeParser<{typeSymbol}>";
            }

            public string GetInitializer(ParserDiscoveryArgs discovery) => $"{discovery}.GetParser<{Symbol}>();";

            public string TryGetInitializer(ParserDiscoveryArgs args, string parserName) => $"return {args}.TryGetParser(out {parserName});";
        }

        private readonly IComponentName TypeParser;

        public TypeParserWrappedDependency(ITypeSymbol typeSymbol) {
            TypeParser = AddComponent(new WrappedTypeParser(typeSymbol));
        }

        protected override string TrySerializeUnsafeSpan(ParsingTypeArgs parsingType, TargetFieldArgs fieldName, BufferArgs destination, OutArgs<int> writtenBytes) {
            return $"return {TypeParser}.TrySerializeUnsafe(in {fieldName}, {destination}, out {writtenBytes});";
        }

        protected override string SerializeUnsafeSpan(ParsingTypeArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination) {
            return $"return {TypeParser}.SerializeUnsafe(in {fieldName}, {destination});";
        }

        protected override string TryDeserializeSpan(ParsingTypeArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes, OutArgs<object> result) {
            return $"return {TypeParser}.TryDeserialize({source}, out {readBytes}, out {result});";
        }

        protected override string TryDeserializeSequence(ParsingTypeArgs collectionArgs, BufferArgs source, OutArgs<object> result) {
            return $"return {TypeParser}.TryDeserialize(ref {source}, out {result});";
        }

        protected override string DeserializeSpan(ParsingTypeArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes) {
            return $"return {TypeParser}.Deserialize({source}, out {readBytes});";
        }

        protected override string DeserializeSequence(ParsingTypeArgs collectionArgs, BufferArgs source) {
            return $"return {TypeParser}.Deserialize(ref {source});";
        }

        protected override string GetLengthFunction(ParsingTypeArgs collectionArgs, TargetFieldArgs fieldName) {
            return $"return {TypeParser}.GetLength(in {fieldName});";
        }
    }
}
