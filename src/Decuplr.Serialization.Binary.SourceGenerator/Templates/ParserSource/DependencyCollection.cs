using System;
using System.Collections.Generic;
using Decuplr.Serialization.Analyzer.BinaryFormat;

namespace Decuplr.Serialization.Binary.Templates.ParserSource {
    internal class DependencyCollection {

        private readonly Dictionary<MemberFormatInfo, ParserDependencyPrecusor> Dependencies = new Dictionary<MemberFormatInfo, ParserDependencyPrecusor>();

        public string StructName { get; }

        public DependencyCollection(string structName, TypeFormatLayout layout) {
            StructName = structName;
        }

        // Get's the name of the parser dep. for the member
        public string this[MemberFormatInfo info] => Dependencies[info];
    }

    internal class ParserDependency {

    }

    interface IParserTransform {
        string TrySerializeUnsafeSpan(ParsingTypeArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination, OutArgs<int> writtenBytes);
        string SerializeUnsafeSpan(ParsingTypeArgs collectionArgs, TargetFieldArgs fieldName, BufferArgs destination);
        string TryDeserializeSpan(ParsingTypeArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes, OutArgs<object> result);
        string TryDeserializeSequence(ParsingTypeArgs collectionArgs, BufferArgs source, OutArgs<object> result);
        string DeserializeSpan(ParsingTypeArgs collectionArgs, BufferArgs source, OutArgs<int> readBytes);
        string DeserializeSequence(ParsingTypeArgs collectionArgs, BufferArgs source);
        string GetLengthFunction(ParsingTypeArgs collectionArgs, TargetFieldArgs fieldName);
    }

    interface IParserTransformFunction : IParserTransform { }

    interface IParserTransformBody : IParserTransform { }


    interface IParserConditionTransformProvider {
        IParserTransformBody GetTransform(IParserTransformFunction transform);
    }
}
