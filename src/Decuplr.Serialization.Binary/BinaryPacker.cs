using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.Serialization.Binary.Generic;
using Decuplr.Serialization.Binary.Internal;

namespace Decuplr.Serialization.Binary {

    public interface IBinaryPacker {
        bool TryGetParser<T>(out TypeParser<T> parser);
        bool TryGetGenericParserProvider(Type genericType, out GenericParserProvider provider);
    }

    public interface INamespaceProvider {
        IBinaryPacker GetNamespace(string parserNamespace);
    }

    // Notes : If a 
    public abstract class BinaryPacker : IBinaryPacker, INamespaceProvider {

        public abstract void AddParserProvider<T>(Func<IBinaryPacker, INamespaceProvider, TypeParser<T>> parserProvider);
        public abstract void AddGenericParserProvider<T>(Type genericType, T provider) where T : GenericParserProvider;
        public abstract void AddSealedParser<T>(TypeParser<T> parser);

        public abstract int Serialize<T>(T value, Span<byte> destination);
        public abstract T Deserialize<T>(ReadOnlySpan<byte> source, out int bytesRead);

        public abstract int GetBinaryLength<T>(T value);
        public abstract int GetBinaryLength<T>(ReadOnlySpan<T> value);

        public abstract bool TryGetParser<T>(out TypeParser<T> parser);
        public abstract bool TryGetGenericParserProvider(Type genericType, out GenericParserProvider provider);

        public abstract IBinaryPacker GetNamespace(string parserNamespace);

        public static BinaryPacker Shared { get; } = new DefaultBinaryFormatter(true);
        public static BinaryPacker Create(bool includeDefaultSerializers) => new DefaultBinaryFormatter(includeDefaultSerializers);
    }

}
