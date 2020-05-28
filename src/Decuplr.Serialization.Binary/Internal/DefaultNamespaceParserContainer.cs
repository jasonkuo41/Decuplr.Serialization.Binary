using System.Collections.Concurrent;
using System.Reflection;
using Decuplr.Serialization.Binary.Internal;
using Decuplr.Serialization.Binary.Namespaces;

namespace Decuplr.Serialization.Binary {
    internal class DefaultNamespaceParserContainer : ParserContainer, IDefaultParserNamespace {

        private readonly ConcurrentDictionary<Assembly, bool> TraversedAssemblies = new ConcurrentDictionary<Assembly, bool>();
        private readonly BinaryPacker Packer;

        public DefaultNamespaceParserContainer(BinaryPacker packer) : base("Default", packer) {
            Packer = packer;
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

        // TODO : Check recursive schema
        public TypeParser<T> GetParser<T>() {
            // If we unable to locate 
        }

        public bool TryGetParser<T>(out TypeParser<T> parser) {

        }
    }
}
