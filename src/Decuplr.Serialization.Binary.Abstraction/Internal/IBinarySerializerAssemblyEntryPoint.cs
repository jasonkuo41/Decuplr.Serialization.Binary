using System.ComponentModel;

namespace Decuplr.Serialization.Binary.Internal {
    /// <summary>
    /// Defines the common entry point of the binary serializer
    /// </summary>
    /// <remarks>
    /// <b>This interface should not be used directly.</b>
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IBinarySerializerAssemblyEntryPoint {
        /// <summary>
        /// Loads the context produced by the assembly
        /// </summary>
        /// <param name="discovery">The discovery service passed for realizing the serializers</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        void LoadAssemblyContext(IBinaryNamespaceDiscovery discovery);
    }
}
