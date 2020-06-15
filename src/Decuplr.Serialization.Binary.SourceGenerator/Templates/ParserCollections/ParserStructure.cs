using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.Templates.ParserCollections {
    internal class ParserCollectionStruct {
        public string StructName { get; }

        public List<MemberParsingTemplate> Parsers { get; } = new List<MemberParsingTemplate>();
    }

    internal struct ParserCollectionArgs {
        public string Name { get; set; }

        public static implicit operator string(ParserCollectionArgs args) => args.Name;
    }

    internal struct TargetFieldArgs {
        public string Name { get; set; }

        public static implicit operator string(TargetFieldArgs args) => args.Name;
    }

    internal struct BufferArgs {
        public string Name { get; set; }

        public static implicit operator string(BufferArgs args) => args.Name;
    }

    internal struct OutArgs<T> {
        public string Name { get; set; }

        public static implicit operator string(OutArgs<T> args) => args.Name;
    }

    internal delegate string InitializeFieldDelegate(string parserDiscovery);

    internal struct ParserDiscoveryArgs {
        public string Name { get; set; }
    }

    internal interface IParserTypeName {
        string TypeName { get; }
        string ParserName { get; }

        string GetInitializer(ParserDiscoveryArgs args);

        string GetInitializer(ParserDiscoveryArgs args, string parserName);
    }

    // Every 
    internal abstract class MemberParsingTemplate {

        protected static string MakeUniqueIndex(int index, params int[] subIndexes) {
            if (subIndexes is null || subIndexes.Length == 0)
                return $"{index}";
            return $"{index}_{string.Join("_", subIndexes.Select(x => x.ToString()))}";
        }

        // The argument name : public TypeParser<DateTime> `parser_1_0`
        public abstract IReadOnlyList<IParserTypeName> Members { get; }

        // bool TrySerializeUnsafe(in T value, Span<byte> dest, out int written);
        // Included as : bool TrySerialize{FieldName}(in ArgsCollection {collectionArgs}, in T {fieldName}, Span<byte> {destination}, out int {writtenBytes})
        public abstract string GetTrySerializeUnsafeSpan(ParserCollectionArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination, OutArgs<int> writtenBytes);

        // int SerializeUnsafe(in T value, Span<byte> writer);
        public abstract string GetSerializeUnsafeSpan(ParserCollectionArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination);

        // DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int read, out T result);
        public abstract string GetTryDeserializeSpan(ParserCollectionArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes, OutArgs<object> result);

        // DeserializeResult TryDeserialize(ref SequenceCursor<byte> cursor);
        public abstract string GetTryDeserializeSequence(ParserCollectionArgs collectionArgs, BufferArgs source, OutArgs<object> result);

        // T Deserialize(ReadOnlySpan<byte> span, out int readBytes);
        public abstract string GetDeserializeSpan(ParserCollectionArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes);

        // T Deserialize(ref SequenceCursor<byte> cursor);
        public abstract string GetDeserializeSequence(ParserCollectionArgs collectionArgs, BufferArgs source);

        // int GetLength(in T value);
        public abstract string GetLengthFunction(ParserCollectionArgs collectionArgs, TargetFieldArgs fieldName);

    }

    internal class TypeParserTemplate : MemberParsingTemplate {

        private class NativeTypeParserType : IParserTypeName {

            private readonly ITypeSymbol Symbol;

            public string TypeName { get; }

            public string ParserName { get; }

            public NativeTypeParserType(ITypeSymbol typeSymbol, int index, params int[] subIndexes) {
                Symbol = typeSymbol;
                TypeName = $"TypeParser<{typeSymbol}>";
                ParserName = $"parser_{MakeUniqueIndex(index, subIndexes)}";
            }

            public string GetInitializer(ParserDiscoveryArgs discovery) => $"{discovery}.GetParser<{Symbol}>();";

            public string GetInitializer(ParserDiscoveryArgs args, string parserName) => $"return {args}.TryGetParser(out {parserName});";
        }

        private readonly IParserTypeName Name;

        public override IReadOnlyList<IParserTypeName> Members { get; }

        public TypeParserTemplate(ITypeSymbol typeSymbol, int index, params int[] subIndexes) {
            Name = new NativeTypeParserType(typeSymbol, index, subIndexes);
            Members = new IParserTypeName[] { Name };
        }

        public override string GetTrySerializeUnsafeSpan(ParserCollectionArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination, OutArgs<int> writtenBytes) {
            return $"return {collectionArgs}.{Name.ParserName}.TrySerializeUnsafe(in {fieldName}, {destination}, out {writtenBytes});";
        }

        public override string GetSerializeUnsafeSpan(ParserCollectionArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination) {
            return $"return {collectionArgs}.{Name.ParserName}.SerializeUnsafe(in {fieldName}, {destination});";
        }

        public override string GetTryDeserializeSpan(ParserCollectionArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes, OutArgs<object> result) {
            return $"return {collectionArgs}.{Name.ParserName}.TryDeserialize({source}, out {readBytes}, out {result});";
        }

        public override string GetTryDeserializeSequence(ParserCollectionArgs collectionArgs, BufferArgs source, OutArgs<object> result) {
            return $"return {collectionArgs}.{Name.ParserName}.TryDeserialize(ref {source}, out {result});";
        }

        public override string GetDeserializeSpan(ParserCollectionArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes) {
            return $"return {collectionArgs}.{Name.ParserName}.Deserialize({source}, out {readBytes});";
        }

        public override string GetDeserializeSequence(ParserCollectionArgs collectionArgs, BufferArgs source) {
            return $"return {collectionArgs}.{Name.ParserName}.Deserialize(ref {source});";
        }

        public override string GetLengthFunction(ParserCollectionArgs collectionArgs, TargetFieldArgs fieldName) {
            return $"return {collectionArgs}.{Name.ParserName}.GetLength(in {fieldName});";
        }
    }

    // For primitives that are only one-byte wide (bool, byte, sbyte)
    internal class PrimitiveByteTemplate : MemberParsingTemplate {

        private class ByteOrderParserType : IParserTypeName {

            public string TypeName => "ByteOrder";

            public string ParserName { get; }

            public ByteOrderParserType(string parserName) {
                ParserName = parserName;
            }

            public string GetInitializer(ParserDiscoveryArgs discovery) => $"{discovery}.{nameof(IParserDiscovery.BinaryLayout)}";

            public string GetInitializer(ParserDiscoveryArgs discovery, string parsers) {
                var node = new CodeNodeBuilder();
                node.AddStatement($"{parsers} = {discovery}.{nameof(IParserDiscovery.BinaryLayout)};");
                node.AddStatement("return true");
                return node.ToString();
            }
        }

        private readonly IParserTypeName Name;
        private readonly ITypeSymbol TypeSymbol;

        public override IReadOnlyList<IParserTypeName> Members { get; }

        public PrimitiveByteTemplate(ITypeSymbol typeSymbol, int index, params int[] subIndexes) {
            Name = new ByteOrderParserType($"byteOrder_{MakeUniqueIndex(index, subIndexes)}");
            TypeSymbol = typeSymbol;
            Members = new IParserTypeName[] { Name };
        }

        private string ThrowArgOutOfRange(string source) => $"throw new {nameof(ArgumentOutOfRangeException)}(nameof({source}))";

        public override string GetTrySerializeUnsafeSpan(ParserCollectionArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination, OutArgs<int> writtenBytes) {
            return $"{destination}[0] = (byte){fieldName}; return true;";
        }

        public override string GetSerializeUnsafeSpan(ParserCollectionArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination) {
            return $"{destination}[0] = (byte){fieldName}; return true;";
        }

        public override string GetTryDeserializeSpan(ParserCollectionArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes, OutArgs<object> result) {
            var node = new CodeNodeBuilder();
            node.AddStatement($"if ({source}.Length < 1) return {DeserializeResult.InsufficientSize.ToDisplayString()}");
            node.AddStatement($"{result} = ({TypeSymbol}){source}[0]");
            node.AddStatement($"{readBytes} = 1");
            node.AddStatement($"return {DeserializeResult.Success.ToDisplayString()}");
            return node.ToString();
        }

        public override string GetTryDeserializeSequence(ParserCollectionArgs collectionArgs, BufferArgs source, OutArgs<object> result) {
            var node = new CodeNodeBuilder();
            // Since cursor always garuantee some space left in the span, unless it's completed (point to end)
            node.AddStatement($"if ({source}.Completed) return {DeserializeResult.InsufficientSize.ToDisplayString()}");
            node.AddStatement($"{result} = ({TypeSymbol}){source}.UnreadSpan[0]");
            node.AddStatement($"{source}.Advance(1)");
            node.AddStatement($"return {DeserializeResult.Success.ToDisplayString()}");
            return node.ToString();
        }

        public override string GetDeserializeSpan(ParserCollectionArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes) {
            var node = new CodeNodeBuilder();
            node.AddStatement($"if ({source}.Length < 1) {ThrowArgOutOfRange(source.ToString())}");
            node.AddStatement($"{readBytes} = 1");
            node.AddStatement($"return ({TypeSymbol}){source}[0]");
            return node.ToString();
        }

        public override string GetDeserializeSequence(ParserCollectionArgs collectionArgs, BufferArgs source) {
            var node = new CodeNodeBuilder();
            node.AddStatement($"if ({source}.IsCompleted) {ThrowArgOutOfRange(source.ToString())}");
            node.AddStatement($"var tempResult = ({TypeSymbol}){source}.UnreadSpan[0]");
            node.AddStatement($"{source}.Advance(1)");
            node.AddStatement($"return tempResult");
            return node.ToString();
        }

        public override string GetLengthFunction(ParserCollectionArgs collectionArgs, TargetFieldArgs fieldName) => "return 1;";
    }
}
