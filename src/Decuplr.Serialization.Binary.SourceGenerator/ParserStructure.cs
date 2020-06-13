using System;
using System.Collections.Generic;
using System.Text;

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

        public List<EnlistedParsers> Parsers { get; } = new List<EnlistedParsers>();
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

    // Every 
    abstract class EnlistedParsers {

        protected EnlistedParsers(string argName) {
            ParserArgName = argName;
        }

        // The argument name : public TypeParser<DateTime> `parser_1_0`
        public string ParserArgName { get; }

        // The type name : public `TypeParser<DateTime>` parser_1_0
        public abstract string ParserTypeName { get; }

        // bool TrySerializeUnsafe(in T value, Span<byte> dest, out int written);
        // Included as : bool TrySerialize{FieldName}(in ArgsCollection {collectionArgs}, in T {fieldName}, Span<byte> {destination}, out int {writtenBytes})
        public abstract string GetTrySerializeUnsafeSpan(ParserCollectionArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination, OutArgs<int> writtenBytes);

        // bool TrySerialize(in T value, IBufferWriter<byte> writer);
        // Included as : bool TrySerialize{FieldName}(in ArgsCollection {collectionArgs}, in T {fieldName}, IBufferWriter<byte> {destination}, out int {writtenBytes})
        public abstract string GetTrySerializeBufferWriter(ParserCollectionArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination);

        // int SerializeUnsafe(in T value, Span<byte> writer);
        public abstract string GetSerializeUnsafeSpan(ParserCollectionArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination);

        // void Serialize(in T value, IBufferWriter<byte> writer);
        public abstract string GetSerializeBufferWriter(ParserCollectionArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination);

        // DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int read, out T result);
        public abstract string GetTryDeserializeSpan(ParserCollectionArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes, OutArgs<object> result);

        // DeserializeResult TryDeserialize(in ReadOnlySequence<byte> seq, out SequencePosition consumed);
        public abstract string GetTryDeserializeSequence(ParserCollectionArgs collectionArgs, BufferArgs source, OutArgs<SequencePosition> consumed);

        // T Deserialize(ReadOnlySpan<byte> span, out int readBytes);
        public abstract string GetDeserializeSpan(ParserCollectionArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes);

        // T Deserialize(in ReadOnlySequence<byte> sequence, out SequencePosition consumed);
        public abstract string GetDeserializeSequence(ParserCollectionArgs collectionArgs, BufferArgs source, OutArgs<SequencePosition> consumed);

        // int GetLength(in T value);
        public abstract string GetLengthFunction(ParserCollectionArgs collectionArgs, TargetFieldArgs fieldName);
    }

    internal class TypeParserArgument : EnlistedParsers {
        public TypeParserArgument(string argName) : base(argName) { }

        public override string ParserTypeName => $"TypeParser<{}>";
    }
}
