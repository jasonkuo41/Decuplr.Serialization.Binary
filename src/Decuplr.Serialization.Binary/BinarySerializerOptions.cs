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

        public static INamespaceTree SetupDefaultNamespaces() {

        }

        public CircularReferenceMode CircularReferenceMode { get; }

        public IBinaryNamespaceDiscovery InitialDiscovery { get; }

        public BinarySerializerOptions(CircularReferenceMode referenceMode, IBinaryNamespaceDiscovery discovery) {
            CircularReferenceMode = referenceMode;
            InitialDiscovery = discovery;
        }
    }

    public static class NamespaceDiscoveryExtensions {
        public static IBinaryNamespaceDiscovery CreateBinaryDiscovery(this IReadOnlyNamespaceTree tree) {

        }
    }
}
