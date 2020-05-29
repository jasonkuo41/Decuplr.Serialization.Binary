using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Decuplr.Serialization.Binary.Namespaces;

namespace Decuplr.Serialization.Binary {
    // Consider caching strategy where we release objects that are no longer in use
    internal class ParserContainer : IParserNamespaceOwner {

        private readonly ConcurrentDictionary<Type, object> Parsers = new ConcurrentDictionary<Type, object>();

        public string Name { get; }
        public LengthProvider? LengthProvider { get; set; }

        public ParserContainer(string containerName) {
            Name = containerName;
        }

        #region Acquiring Parser

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TypeParser<T> GetGenericParser<T>(Type type, IParserDiscovery parserNamespace) {
            var genericType = type.GetGenericTypeDefinition();
            if (!Parsers.TryGetValue(genericType, out var provider))
                throw new ParserNotFoundException($"Unable to locate generic type parser '{genericType}' for {type}.", genericType);
            var gProvider = provider as GenericParserProvider;
            Debug.Assert(provider != null);

            // Since the parser is sucessfully created, we can cache the result (as IParserProvider<T>)
            var parser = gProvider.ProvideParser<T>(parserNamespace);
            Parsers.TryAdd(type, gProvider.CreateProvider<T>());
            return parser;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetNonGenericParser<T>(Type type, IParserDiscovery parserNamespace, out TypeParser<T> parser) {
            parser = null;
            if (!Parsers.TryGetValue(type, out var regParser))
                return false;

            switch (regParser) {
                case TypeParser<T> sealedParser:
                    parser = sealedParser;
                    return true;
                case IParserProvider<T> parserProvider:
                    return parserProvider.TryProvideParser(parserNamespace, out parser);
            }

            Debug.Fail($"{regParser} should be registered for {type.Name}");
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetGenericParser<T>(Type type, IParserDiscovery parserNamespace, out TypeParser<T> parser) {
            parser = null;
            var genericType = type.GetGenericTypeDefinition();
            if (!Parsers.TryGetValue(genericType, out var provider))
                return false;
            var gProvider = provider as GenericParserProvider;
            Debug.Assert(provider != null);
            if (!gProvider.TryProvideParser(parserNamespace, out parser))
                return false;
            // Cache the parser for this type
            Parsers.TryAdd(type, gProvider.CreateProvider<T>());
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TypeParser<T> GetNonGenericParser<T>(Type type, IParserDiscovery parserNamespace) {
            if (!Parsers.TryGetValue(type, out var regParser))
                throw ParserNotFound();

            switch (regParser) {
                case TypeParser<T> sealedParser: return sealedParser;
                case IParserProvider<T> parserProvider: return parserProvider.ProvideParser(parserNamespace);
            }
            // That's odd
            Debug.Fail($"{regParser} should be registered for {type.Name}");
            throw ParserNotFound(); ;

            Exception ParserNotFound() => new ParserNotFoundException($"Unable to locate parser for type {type}", type);
        }

        public virtual TypeParser<T> GetParser<T>(IParserDiscovery discovery) {
            var type = typeof(T);
            if (type.IsGenericType) {
                // Attempt to get a solid parser first
                // TODO : should we change the behaviour to make the IParserProvider actually throw what it is missing?
                if (TryGetNonGenericParser<T>(type, discovery, out var parser))
                    return parser;

                // TODO : Cache the result of this generic parser!
                // Otherwise, forcefully find the generic parser
                return GetGenericParser<T>(type, discovery);
            }
            return GetNonGenericParser<T>(type, discovery);
        }

        public virtual bool TryGetParser<T>(IParserDiscovery discovery, out TypeParser<T> parser) {
            var type = typeof(T);
            // See if it's a generic type and apply the following the rules
            if (type.IsGenericType) {
                // Attempt to get a solid parser first, for example, if someone implement List<int> rather then List<>, we would look up that first
                if (TryGetNonGenericParser(type, discovery, out parser))
                    return true;
                // Otherwise, try to locate the generic parser
                return TryGetGenericParser(type, discovery, out parser);
            }
            return TryGetNonGenericParser(type, discovery, out parser);
        }

        #endregion

        private void CheckGenericType(Type genericType) {
            if (!genericType.IsGenericTypeDefinition)
                throw new ArgumentException("Can only accept types that are generics and only is generic type definition", nameof(genericType));
        }

        public virtual bool AddParserProvider<TProvider, TType>(TProvider provider) where TProvider : IParserProvider<TType> => Parsers.TryAdd(typeof(TType), provider);
        public virtual bool AddSealedParser<T>(TypeParser<T> parser) => Parsers.TryAdd(typeof(T), parser);
        public virtual bool AddGenericParserProvider<TParser>(Type genericType) where TParser : GenericParserProvider {
            CheckGenericType(genericType);
            return Parsers.TryAdd(genericType, typeof(TParser));
        }

        public virtual void ReplaceParserProvider<TProvider, TType>(TProvider provider) where TProvider : IParserProvider<TType> => Parsers[typeof(TType)] = provider;
        public virtual void ReplaceSealedParser<T>(TypeParser<T> parser) => Parsers[typeof(T)] = parser;
        public virtual void ReplaceGenericParserProvider<TParser>(Type genericType) where TParser : GenericParserProvider {
            CheckGenericType(genericType);
            Parsers[genericType] = typeof(TParser);
        }

    }
}
