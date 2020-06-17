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

    internal interface IParserConditionTransform {
        // bool TryDeserialize(in TSource value, ReadOnlySpan<byte> span, out int writtenBytes, out T result, out DeserializeResult result)
        string TryDeserialize(string original, ParsingTypeArgs typeArgs, BufferArgs destination, OutArgs<int> writtenBytes, OutArgs<object> value, OutArgs<DeserializeResult> result);

    }
}
