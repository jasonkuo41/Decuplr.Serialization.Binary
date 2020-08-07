using System;

namespace Decuplr.Serialization.Binary {
    internal static class SerializationPrimitivesExtension {
        internal static string ToDisplayString(this DeserializeResult result) => result.Conclusion switch
        {
            DeserializeConclusion.Success => $"{nameof(DeserializeResult)}.{nameof(DeserializeResult.Success)}",
            DeserializeConclusion.Faulted => $"{nameof(DeserializeResult)}.{nameof(DeserializeResult.Faulted)}",
            DeserializeConclusion.InsufficientSize => $"{nameof(DeserializeResult)}.{nameof(DeserializeResult.InsufficientSize)}",
            _ => throw new ArgumentException("Invalid conclusion type")
        };
    }
}
