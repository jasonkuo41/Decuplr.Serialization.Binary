using System;
using System.Text;
using Decuplr.Serialization.Binary.Generic;

namespace Decuplr.Serialization.Binary {

    public interface IBinaryFormatter {
        bool TryGetParser<T>(out TypeParser<T> parser);
        bool TryGetGenericParserProvider<T>(out GenericParserProvider provider);
    }

    public interface INamespaceProvider {
        IBinaryFormatter GetNamespace(string parserNamespace);
    }

    // Notes : If a 
    public abstract class BinaryFormatter : IBinaryFormatter, INamespaceProvider {

        public abstract void AddParserProvider<T>(Func<IBinaryFormatter, INamespaceProvider, TypeParser<T>> parserProvider);
        public abstract void AddGenericParserProvider<T>(Type genericType, T provider) where T : GenericParserProvider;
        public abstract void AddSealedParser<T>(TypeParser<T> parser);

        public abstract int Serialize<T>(T value, Span<byte> destination);
        public abstract T Deserialize<T>(ReadOnlySpan<byte> source, out int bytesRead);

        public abstract int GetBinaryLength<T>(T value);
        public abstract int GetBinaryLength<T>(ReadOnlySpan<T> value);

        public abstract bool TryGetParser<T>(out TypeParser<T> parser);
        public abstract bool TryGetGenericParserProvider<T>(out GenericParserProvider provider);

        public abstract IBinaryFormatter GetNamespace(string parserNamespace);

        public static BinaryFormatter Shared { get; } = new DefaultBinaryFormatter(true);
        public static BinaryFormatter Create(bool includeDefaultSerializers) => new DefaultBinaryFormatter(includeDefaultSerializers);
    }
}
