using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Decuplr.Serialization.Binary {
    internal class ParserDiscovery : IParserDiscovery {

        // We can detect if there's a cicular reference with this bad boy
        // TODO : Add a fuctionatily where we can point back to the index of certain object, "It will be complicated"
        private class ParserDiscoveryProxy : IParserDiscovery {

            private readonly HashSet<Type> TraversedType;
            private readonly bool ThrowOnCircular;

            private ParserDiscovery CurrentParent;

            public LengthProvider LengthProvider => CurrentParent.LengthProvider;

            public ParserDiscoveryProxy(ParserDiscovery locator, bool throwOnCircular = false) {
                CurrentParent = locator;
                ThrowOnCircular = throwOnCircular;
            }

            private ParserDiscoveryProxy (ParserDiscoveryProxy proxy) {
                CurrentParent = proxy.CurrentParent;
                TraversedType = new HashSet<Type>(proxy.TraversedType, proxy.TraversedType.Comparer);
                ThrowOnCircular = proxy.ThrowOnCircular;
            }

            private Exception DetectedCircularException(Type locateType) => new CircularSchemaReferenceException("Detected circular schema reference, parser cannot be created.", locateType);

            public TypeParser<T> GetParser<T>() {
                var nextProxy = new ParserDiscoveryProxy(this);
                var locateType = typeof(T);
                if (!nextProxy.TraversedType.Add(locateType))
                    throw DetectedCircularException(locateType);
                return CurrentParent.GetParser<T>(nextProxy);
            }

            // TODO : Should the circularschemareference throw here too? and not silently fail?
            public bool TryGetParser<T>(out TypeParser<T> parser) {
                parser = null;
                var nextProxy = new ParserDiscoveryProxy(this);
                var locateType = typeof(T);
                if (nextProxy.TraversedType.Add(locateType))
                    return CurrentParent.TryGetParser(nextProxy, out parser);
                if (ThrowOnCircular)
                    throw DetectedCircularException(locateType);
                return false;
            }

            public IParserDiscovery WithNamespace(IEnumerable<string> parserNamespace, bool shouldPrioritize = false) {
                CurrentParent = (ParserDiscovery)CurrentParent.WithNamespace(parserNamespace, shouldPrioritize);
                return this;
            }

            public IParserDiscovery WithNamespace(string parserNamespace, bool shouldPrioritize = false) {
                CurrentParent = (ParserDiscovery)CurrentParent.WithNamespace(parserNamespace, shouldPrioritize);
                return this;
            }
        }

        private readonly ConcurrentDictionary<Type, TypeParser> CachedParser = new ConcurrentDictionary<Type, TypeParser>();
        private readonly ParserNamespaces DiscoverNamespaces;
        private readonly ParserContainer[] CurrentNamespace = Array.Empty<ParserContainer>();
        private readonly ParserContainer[] PrioritizedNamespace = Array.Empty<ParserContainer>();

        public LengthProvider LengthProvider {
            get {
                if (TryGetProvider(PrioritizedNamespace, out var provider))
                    return provider;
                if (TryGetProvider(CurrentNamespace, out provider))
                    return provider;
                return DiscoverNamespaces.Default.LengthProvider;

                static bool TryGetProvider(ParserContainer[] containers, out LengthProvider provider) {
                    for (var i = 0; i < containers.Length; ++i) {
                        provider = containers[i].LengthProvider;
                        if (provider != null)
                            return true;
                    }
                    provider = null;
                    return false;
                }
            }
        }

        public ParserDiscovery(ParserNamespaces namespaces) {
            DiscoverNamespaces = namespaces;
        }

        public ParserDiscovery(ParserNamespaces namespaces, IEnumerable<string> targetNamespaces) : this(namespaces) {
            CurrentNamespace = GetContainers(namespaces, targetNamespaces).ToArray();
        }

        public ParserDiscovery(ParserNamespaces namespaces, IEnumerable<string> targetNamespaces, IEnumerable<string> prioritizeNamespace) : this(namespaces, targetNamespaces) {
            PrioritizedNamespace = GetContainers(namespaces, prioritizeNamespace).ToArray();
        }

        public ParserDiscovery(ParserDiscovery discovery, IEnumerable<string> targetNamespaces, IEnumerable<string> priotizeNamespace)
            : this (discovery.DiscoverNamespaces) {
            // Assume namespaces are added in order by their number
            // [First] P1 P3 (Prioritized) | S5 S4 S2 [Last]

            // If it's not prioritized, we stack the namespace, so it get's discovered first
            CurrentNamespace = GetContainers(discovery.DiscoverNamespaces, targetNamespaces).Concat(discovery.CurrentNamespace).ToArray();

            // Otherwise we queue the namespace to the prioritized group, so it get's discovered later
            PrioritizedNamespace = discovery.PrioritizedNamespace.Concat(GetContainers(discovery.DiscoverNamespaces, priotizeNamespace)).ToArray();
        }

        public ParserDiscovery(ParserDiscovery discovery, string targetNamespace, string priotizeNamespace) 
            : this (discovery.DiscoverNamespaces) {

            var currentNamespace 
        }

        private static IEnumerable<ParserContainer> GetContainers(ParserNamespaces namespaces, IEnumerable<string> targetNamespaces) {
            return targetNamespaces.Select(target => {
                if (!namespaces.TryGetNamespace(target, out var container))
                    throw new ArgumentException($"Unable to locate namespace `{target}`.", nameof(targetNamespaces));
                return container;
            });
        }

        private bool TryGetNonDefaultParser<T>(IParserDiscovery parserNamespace, out TypeParser<T> parser) {
            if (TryGetParserInternal(PrioritizedNamespace, out parser))
                return true;
            if (TryGetParserInternal(CurrentNamespace, out parser))
                return true;
            parser = null;
            return false;

            bool TryGetParserInternal(ParserContainer[] containers, out TypeParser<T> parser) {
                for (var i = 0; i < containers.Length; ++i) {
                    if (containers[i].TryGetParser(parserNamespace, out parser))
                        return true;
                }
                parser = null;
                return false;
            }
        }

        private TypeParser<T> GetParser<T>(IParserDiscovery travesor) {
            if (CachedParser.TryGetValue(typeof(T), out var uncastParser))
                return (TypeParser<T>)uncastParser;
            var parser = ActualGetParser(travesor);
            CachedParser.TryAdd(typeof(T), parser);
            return parser;

            // Actually retreive the parser and not rely on the cache
            TypeParser<T> ActualGetParser(IParserDiscovery travesor) {
                // First we traverse all sub namespaces
                if (TryGetNonDefaultParser<T>(travesor, out var parser))
                    return parser;
                // Finally we lookup to the default Namespace
                return DiscoverNamespaces.Default.GetParser<T>(travesor);
            }
        }

        private bool TryGetParser<T>(IParserDiscovery travesor, out TypeParser<T> parser) {
            if (CachedParser.TryGetValue(typeof(T), out var uncastParser)) {
                parser = (TypeParser<T>)uncastParser;
                return true;
            }
            if (!TryActualGetParser(travesor, out parser))
                return false;
            CachedParser.TryAdd(typeof(T), parser);
            return true;

            bool TryActualGetParser(IParserDiscovery travesor, out TypeParser<T> parser) {
                // First we traverse all sub namespaces
                if (TryGetNonDefaultParser(travesor, out parser))
                    return true;
                // Finally we lookup to the default Namespace
                return DiscoverNamespaces.Default.TryGetParser(this, out parser);
            }
        }

        // This demonstrates how we can bypass circular reference check, at the risk of stackoverflow
        internal TypeParser<T> GetParserUnsafe<T>() => GetParser<T>(this);

        internal bool TryGetParserUnsafe<T>(out TypeParser<T> parser) => TryGetParser(this, out parser);

        public TypeParser<T> GetParser<T>() => GetParser<T>(new ParserDiscoveryProxy(this));

        public bool TryGetParser<T>(out TypeParser<T> parser) => TryGetParser(new ParserDiscoveryProxy(this), out parser);
        public bool TryGetParser<T>(bool throwOnCircularRef, out TypeParser<T> parser) => TryGetParser(new ParserDiscoveryProxy(this, throwOnCircularRef), out parser);


        // Assume namespaces are added in order by their number
        // [First] P1 P3 (Prioritized) | S5 S4 S2 [Last]
        public IParserDiscovery WithNamespace(IEnumerable<string> parserNamespace, bool shouldPrioritize = false) {
            // If it's not proritize, we stack the namespace, so it get's discovered first
            if (!shouldPrioritize)
                return new ParserDiscovery(DiscoverNamespaces, parserNamespace.Concat(CurrentNamespace), PrioritizedNamespace);
            // Otherwise we queue the namespace to the priotized group, so it get's discovered later
            return new ParserDiscovery(DiscoverNamespaces, CurrentNamespace, PrioritizedNamespace.Concat(parserNamespace));
        }

        public IParserDiscovery WithNamespace(string parserNamespace, bool shouldPrioritize = false) {
            // If it's not proritize, we stack the namespace, so it get's discovered first
            if (!shouldPrioritize)
                return new ParserDiscovery(DiscoverNamespaces, CurrentNamespace.Prepend(parserNamespace), PrioritizedNamespace);
            // Otherwise we queue the namespace to the priotized group, so it get's discovered later
            return new ParserDiscovery(DiscoverNamespaces, CurrentNamespace, PrioritizedNamespace.Append(parserNamespace));
        }
    }
}
