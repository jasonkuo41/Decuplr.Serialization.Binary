using System;
using System.Collections.Generic;
using Decuplr.Serialization.Binary.Arguments;

namespace Decuplr.Serialization.Binary.Schemas {
    interface IDeserializeSolution {
        IReadOnlyList<IGeneratedType> GeneratedTypes { get; }
        string TryDeserializeSpan(ParserConstructArgs construct, BufferArgs buffer, OutArgs<int> readBytes, OutArgs<object> result);
        string TryDeserializeSequence(ParserConstructArgs construct, BufferArgs buffer, OutArgs<object> result);
        string DeserializeSpan(ParserConstructArgs construct, BufferArgs buffer, OutArgs<int> readBytes);
        string DeserializeSequence(ParserConstructArgs construct, BufferArgs buffer);
    }

}
