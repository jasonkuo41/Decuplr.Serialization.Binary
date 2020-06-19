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

    /// <summary>
    /// Provides a set of base functions for the parser dependency
    /// </summary>
    interface IParsingConditions {
        string TrySerializeUnsafeSpan(string nextFunction, ParsingTypeArgs targetType, TargetFieldArgs fieldName, BufferArgs destination, OutArgs<int> writtenBytes);
        string SerializeUnsafeSpan(string nextFunction, ParsingTypeArgs targetType, TargetFieldArgs fieldName, BufferArgs destination);
        string TryDeserializeSpan(string nextFunction, ParsingTypeArgs targetType, BufferArgs source, OutArgs<int> readBytes, OutArgs<object> result);
        string TryDeserializeSequence(string nextFunction, ParsingTypeArgs targetType, BufferArgs source, OutArgs<object> result);
        string DeserializeSpan(string nextFunction, ParsingTypeArgs targetType, BufferArgs source, OutArgs<int> readBytes);
        string DeserializeSequence(string nextFunction, ParsingTypeArgs targetType, BufferArgs source);
        string GetLengthFunction(string nextFunction, ParsingTypeArgs targetType, TargetFieldArgs fieldName);
    }

}
