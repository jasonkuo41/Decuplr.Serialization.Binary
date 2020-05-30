using System;
using Decuplr.Serialization.Binary.Namespaces;

namespace Decuplr.Serialization.Binary.Internal.DefaultParsers {

    internal class MrBoilerPlate<T> : GenericParserProvider {

        /// Note currently we parse the underlying daya asap,
        /// But what we actually can do is store a block of memory and only serialize that value when it's evaluated, basically, 
        /// We need 
        ///  (1) the class to implement IDisposable (so we can recycle the underlying memory) (or not, depends how the user implement it's class)
        ///  (2) able to determinate the layout with the nested type. For example list, then we must know how big the list is
        ///      , so we can skip that part and serialize the next part.
        /// 
        /// Thoughts:
        ///     This approach would be a bit tedious, and we need to know the underlying size of the object 
        ///     (unless we encode the object with a length info telling use how big this block is)
        ///     otherwise we still maybe need to 
        ///         (1) travel through every point to get the length info and sum together, which might be counter productive
        ///         (2) if we have a bug in our code, that would could be a disater as we misallign everything
        ///  
        private class LazyTypeParser : TypeParser<Lazy<T>> {

            private readonly TypeParser<T> ChildParser;

            public LazyTypeParser(TypeParser<T> childParser) {
                ChildParser = childParser;
            }

            public LazyTypeParser(IParserDiscovery formatter) {
                if (!formatter.TryGetParser(out ChildParser))
                    throw new NotSupportedException($"Unable to locate type parser for such underlying type {typeof(T).Name}");
            }

            public override int GetBinaryLength(Lazy<T> value) {
                return ChildParser.GetBinaryLength(value.Value);
            }

            public override DeserializeResult TryDeserialize(ReadOnlySpan<byte> span, out int readBytes, out Lazy<T> result) {
                result = null;
                var parseResult = ChildParser.TryDeserialize(span, out readBytes, out var nestedResult);
                if (parseResult != DeserializeResult.Success)
                    return parseResult;
                result = new Lazy<T>(() => nestedResult);
                return DeserializeResult.Success;
            }

            public override bool TrySerialize(Lazy<T> value, Span<byte> destination, out int writtenBytes)
                => ChildParser.TrySerialize(value.Value, destination, out writtenBytes);

        }

        public override TypeParser ProvideParser(IParserDiscovery formatter) {
            return new LazyTypeParser(formatter);
        }

        public override bool TryProvideParser(IParserDiscovery formatter, out TypeParser parser) {
            throw new NotImplementedException();
        }
    }
}
