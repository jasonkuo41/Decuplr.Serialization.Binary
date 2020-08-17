using System.Linq;
using System.Reflection;
using Decuplr.Serialization.Namespaces;

namespace Decuplr.Serialization.Binary {
    public class BinarySerializerOptions {

        public static BinarySerializerOptions Default { get; }

        public static BinarySerializerOptions Performance { get; }

        public static IReadOnlyNamespaceTree DefaultNamespaces { get; }

        static BinarySerializerOptions() {
            DefaultNamespaces = SetupDefaultNamespaces();
            Default = new BinarySerializerOptions(CircularReferenceMode.DetectAndThrow, DefaultNamespaces.CreateBinaryDiscovery());
            Performance = new BinarySerializerOptions(CircularReferenceMode.NeverDetect, DefaultNamespaces.CreateBinaryDiscovery());
        }

        public CircularReferenceMode CircularReferenceMode { get; }

        public IBinaryNamespaceDiscovery InitialDiscovery { get; }

        public BinarySerializerOptions(CircularReferenceMode referenceMode, IBinaryNamespaceDiscovery discovery) {
            CircularReferenceMode = referenceMode;
            InitialDiscovery = discovery;
        }

        private static INamespaceTree SetupDefaultNamespaces() {
            var tree = NamespaceTree.Create();
            foreach (var assembly in Assembly.GetEntryAssembly().GetReferencedAssemblies().Where(x => x))
                tree.AddBinarySerializers(assembly);
            return tree;
        }

    }
}
