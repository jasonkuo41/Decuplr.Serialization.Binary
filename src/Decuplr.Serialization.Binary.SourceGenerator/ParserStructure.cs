using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    // Here we define the parser structure
    // The parser comes in with three different places
    // 1. Additional partial type to the og type
    // 2. 
    // 3. 

    // This file can then be finalized into actual code
    class ParserMainStructure {

    }

    class ParserArgsStructure {
        public string StructTypeName { get; }

        public List<MemberParsingTemplate> Parsers { get; } = new List<MemberParsingTemplate>();
    }

    struct ParserCollectionArgs {
        public string Name { get; set; }
    }

    struct TargetFieldArgs {
        public string Name { get; set; }
    }

    struct BufferArgs {
        public string Name { get; set; }
    }

    struct OutArgs<T> {
        public string Name { get; set; }
    }

    struct TypeNamePair {
        public string TypeName { get; set; }
        public string ParserName { get; set; }

        public static implicit operator TypeNamePair((string, string) tuple) => new TypeNamePair {
            ParserName = tuple.Item1,
            TypeName = tuple.Item2
        };
    }

    // Every 
    abstract class MemberParsingTemplate {

        // The argument name : public TypeParser<DateTime> `parser_1_0`
        public abstract IReadOnlyList<TypeNamePair> Parsers { get; }

        public abstract string RetrieveParser { get; }

        // bool TrySerializeUnsafe(in T value, Span<byte> dest, out int written);
        // Included as : bool TrySerialize{FieldName}(in ArgsCollection {collectionArgs}, in T {fieldName}, Span<byte> {destination}, out int {writtenBytes})
        public abstract string GetTrySerializeUnsafeSpan(ParserCollectionArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination, OutArgs<int> writtenBytes);

        // int SerializeUnsafe(in T value, Span<byte> writer);
        public abstract string GetSerializeUnsafeSpan(ParserCollectionArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination);

        // DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int read, out T result);
        public abstract string GetTryDeserializeSpan(ParserCollectionArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes, OutArgs<object> result);

        // DeserializeResult TryDeserialize(in ReadOnlySequence<byte> seq, out long consumed);
        public abstract string GetTryDeserializeSequence(ParserCollectionArgs collectionArgs, BufferArgs source, OutArgs<SequencePosition> consumed);

        // T Deserialize(ReadOnlySpan<byte> span, out int readBytes);
        public abstract string GetDeserializeSpan(ParserCollectionArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes);

        // T Deserialize(in ReadOnlySequence<byte> sequence, out long consumed);
        public abstract string GetDeserializeSequence(ParserCollectionArgs collectionArgs, BufferArgs source, OutArgs<SequencePosition> consumed);

        // int GetLength(in T value);
        public abstract string GetLengthFunction(ParserCollectionArgs collectionArgs, TargetFieldArgs fieldName);
    }

    internal class TypeParserTemplate : MemberParsingTemplate {

        private readonly TypeNamePair Name;

        public override IReadOnlyList<TypeNamePair> Parsers { get; }

        public TypeParserTemplate(ITypeSymbol typeSymbol, int index) {
            Name = ($"TypeParser<{typeSymbol}>", $"parser_{index}");
            Parsers = new TypeNamePair[] { Name };
        }

        public override string GetTrySerializeUnsafeSpan(ParserCollectionArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination, OutArgs<int> writtenBytes) {
            return $"return {collectionArgs}.{Name.ParserName}.TrySerializeUnsafe(in {fieldName}, {destination}, out {writtenBytes})";
        }

        public override string GetSerializeUnsafeSpan(ParserCollectionArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination) {
            return $"return {collectionArgs}.{Name.ParserName}.SerializeUnsafe(in {fieldName}, {destination})";
        }

        public override string GetTryDeserializeSpan(ParserCollectionArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes, OutArgs<object> result) {
            return $"return {collectionArgs}.{Name.ParserName}.TryDeserialize({source}, out {readBytes}, out {result})";
        }

        public override string GetTryDeserializeSequence(ParserCollectionArgs collectionArgs, BufferArgs source, OutArgs<long> consumed) {
            return $"return {collectionArgs}.{Name.ParserName}.TryDeserialize(in {source}, out {consumed})";
        }

        public override string GetDeserializeSpan(ParserCollectionArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes) {
            return $"return {collectionArgs}.{Name.ParserName}.Deserialize({source}, out {readBytes})";
        }

        public override string GetDeserializeSequence(ParserCollectionArgs collectionArgs, BufferArgs source, OutArgs<long> consumed) {
            return $"return {collectionArgs}.{Name.ParserName}.Deserialize(in {source}, out {consumed})";
        }

        public override string GetLengthFunction(ParserCollectionArgs collectionArgs, TargetFieldArgs fieldName) {
            return $"return {collectionArgs}.{Name.ParserName}.GetLength(in {fieldName})";
        }
    }

    // For primitives that are only one-byte wide (bool, byte, sbyte)
    internal class PrimitiveByteTemplate : MemberParsingTemplate {

        private readonly TypeNamePair Name;
        private readonly ITypeSymbol TypeSymbol;

        public override IReadOnlyList<TypeNamePair> Parsers { get; }

        public PrimitiveByteTemplate(ITypeSymbol typeSymbol, int index) {
            Name = ("ByteOrder", $"order_{index}");
            TypeSymbol = typeSymbol;
            Parsers = new TypeNamePair[] { Name };
        }

        public override string GetTrySerializeUnsafeSpan(ParserCollectionArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination, OutArgs<int> writtenBytes) {
            return $"{destination}[0] = (byte){fieldName}; return true;";
        }

    }
}
