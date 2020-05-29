using System;
using System.Collections.Concurrent;
using System.Reflection;
using Decuplr.Serialization.Binary.Internal;
using Decuplr.Serialization.Binary.Namespaces;

namespace Decuplr.Serialization.Binary {
    internal class DefaultNamespaceParserContainer : ParserContainer, IDefaultParserNamespace {

        private static readonly Assembly SystemPrivateCoreLib = typeof(Console).Assembly;

        private readonly ConcurrentDictionary<Assembly, bool> TraversedAssemblies = new ConcurrentDictionary<Assembly, bool>();
        private readonly ParserDiscovery DefaultLocator;
        private readonly BinaryPacker Packer;

        public LengthProvider LengthProvider => DefaultLocator.LengthProvider;

        public DefaultNamespaceParserContainer(BinaryPacker packer, ParserDiscovery defaultLocator) : base("Default", packer) {
            Packer = packer;
            DefaultLocator = defaultLocator;
        }

        private bool TryLoadAssembly<T>() {
            var assembly = typeof(T).Assembly;
            if (TraversedAssemblies.ContainsKey(assembly))
                return false;
            var entryPoint = assembly.GetParserEntryPoint();
            if (entryPoint is null)
                return TraversedAssemblies.TryAdd(assembly, false);
            // Load that entry point
            entryPoint.LoadContext(Packer);
            return true;
        }

        public TypeParser<T> GetParser<T>() {
            // If we are unable to locate the parser first time 
            if (DefaultLocator.TryGetParser<T>(throwOnCircularRef: true, out var parser))
                return parser;
            // We load the assembly and if we previously didn't locate it, then we recursively run this function again
            // PS, should we just call the function above use instead :|
            if (TryLoadAssembly<T>())
                return GetParser<T>();
            // Dump detailed exception here
            // If the assembly is System.* then we will say it's probably not a registered parser
            // Otherwise it'll add a hint if the assembly was not compiled with source generator
            var assembly = typeof(T).Assembly;
            // ugh... I feel like this are enough vendors we trust not to do dumb things... I guess?
            // wait... then why is microsoft here?! I shall summon silverlight back with blazor!!!!
            if (assembly == SystemPrivateCoreLib || assembly.FullName.StartsWith("System.") || assembly.FullName.StartsWith("Microsoft."))
                throw new ParserNotFoundException($"Unable to locate parser of type `{typeof(T)}`, you must register the parser before using it.", typeof(T));
            throw new ParserNotFoundException($"Unable to locate parser of type `{typeof(T)}`, you must register the parser before using it. (Did you forgot to run our source generator along with your project?)", typeof(T));
        }

        public bool TryGetParser<T>(out TypeParser<T> parser) {
            // Richard, if you need comment, see above, it's almost the same, just to fail operations so we don't throw
            if (DefaultLocator.TryGetParser(out parser))
                return true;
            if (TryLoadAssembly<T>())
                return TryGetParser(out parser);
            return false;
        }
    }
}
