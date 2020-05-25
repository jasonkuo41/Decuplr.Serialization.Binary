using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary {

    internal class ImplBinarySerializer : BinaryFormatter {

        private readonly Dictionary<string?, Dictionary<Type, object>> Serializers = new Dictionary<string?, Dictionary<Type, object>> {
            { null , new Dictionary<Type, object>() }
        };

        public override void AddFormatter<T>(BinaryParser<T> parser) => Serializers[null].Add(typeof(T), parser);

        public override void AddFormatter<T>(string parserNamespace, BinaryParser<T> parser) {
            if (!Serializers.ContainsKey(parserNamespace))
                Serializers.Add(parserNamespace, new Dictionary<Type, object>());
            Serializers[parserNamespace].Add(typeof(T), parser);
        }

        public override bool TryGetFormatter<T>(out BinaryParser<T> parser) {
            throw new NotImplementedException();
        }

        public override IReadOnlyDictionary<Type, object> GetNamespace(string parserNamespace) {
            throw new NotImplementedException();
        }

        public override void OverrideFormatter<T>(BinaryParser<T> parser) {
            throw new NotImplementedException();
        }

        public override void OverrideFormatter<T>(string parserNamespace, BinaryParser<T> parser) {
            throw new NotImplementedException();
        }

        public override bool TryAddFormatter<T>(BinaryParser<T> parser) {
            throw new NotImplementedException();
        }

        public override bool TryAddFormatter<T>(string parserNamespace, BinaryParser<T> parser) {
            throw new NotImplementedException();
        }
    }

    public interface IBinaryFormatter {
        bool TryGetFormatter<T>(out BinaryParser<T> parser);
    }

    public interface IBinaryNamespace {
        IBinaryFormatter GetNamespace(string parserNamespace);
    }

    public abstract class BinaryFormatter : IBinaryFormatter, IBinaryNamespace {

        public abstract void AddFormatter<T>(BinaryParser<T> parser);
        public abstract void AddFormatter<T>(string parserNamespace, BinaryParser<T> parser);

        public abstract void AddFormatterSource<T>(Func<IBinaryFormatter, IBinaryNamespace, BinaryParser<T>> parserSource);

        public abstract bool TryAddFormatter<T>(BinaryParser<T> parser);
        public abstract bool TryAddFormatter<T>(string parserNamespace, BinaryParser<T> parser);

        public abstract void OverrideFormatter<T>(BinaryParser<T> parser);
        public abstract void OverrideFormatter<T>(string parserNamespace, BinaryParser<T> parser);

        public abstract bool TryGetFormatter<T>(out BinaryParser<T> parser);
        public abstract IBinaryFormatter GetNamespace(string parserNamespace);

        public static BinaryFormatter Shared { get; } = new ImplBinarySerializer();
        public static BinaryFormatter Create() => new ImplBinarySerializer();
    }
}
