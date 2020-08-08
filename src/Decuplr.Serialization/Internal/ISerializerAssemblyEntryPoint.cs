using System.ComponentModel;
using Decuplr.Serialization.Namespaces;

namespace Decuplr.Serialization.Internal {
    /// <summary>
    /// Defines the common entry point of the binary serializer
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface ISerializerAssemblyEntryPoint {
        /// <summary>
        /// Loads the context produced by the assembly
        /// </summary>
        /// <param name="discovery">The discovery service passed for realizing the serializers</param>
        void LoadAssemblyContext(INamespaceDiscovery discovery);
    }
}
