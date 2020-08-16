namespace Decuplr.Serialization.Namespaces {
    /// <summary>
    /// Represents a read only root of the namespaces which contains all nodes that belong to this collection.
    /// </summary>
    public interface IReadOnlyNamespaceTree : IReadOnlyNamespaceNode {

        /// <summary>
        /// Incremented for each modification and can be used to verify cached results.
        /// </summary>
        int Revision { get; }
    }
}
