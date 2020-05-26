using System;
using System.Text;

namespace Decuplr.Serialization.Binary {

    public interface IBinaryFormatter {
        bool TryGetFormatter<T>(out BinaryParser<T> parser); 
        bool TryGetCollectionFormatter<T>(out CollectionParser<T> parser);
    }

    public interface IBinaryNamespace {
        IBinaryFormatter GetNamespace(string parserNamespace);
    }

    // Notes : If a 
    public abstract class BinaryFormatter : IBinaryFormatter, IBinaryNamespace {

        public abstract void AddParserProvider<T>(Func<IBinaryFormatter, IBinaryNamespace, BinaryParser<T>> parserProvider);
        public abstract void AddImmutableParser<T>(BinaryParser<T> parser);

        public abstract int Serialize<T>(T value, Span<byte> destination);
        public abstract T Deserialize<T>(ReadOnlySpan<byte> source, out int bytesRead);

        public abstract int GetBinaryLength<T>(T value);
        public abstract int GetBinaryLength<T>(ReadOnlySpan<T> value);

        public abstract bool TryGetFormatter<T>(out BinaryParser<T> parser);
        public abstract bool TryGetCollectionFormatter<T>(out CollectionParser<T> parser);
        public abstract IBinaryFormatter GetNamespace(string parserNamespace);

        public static BinaryFormatter Shared { get; } = new DefaultBinaryFormatter(true);
        public static BinaryFormatter Create(bool includeDefaultSerializers) => new DefaultBinaryFormatter(includeDefaultSerializers);
    }
}
