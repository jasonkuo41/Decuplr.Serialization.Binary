namespace Decuplr.Serialization.Binary.Annotations.Namespaces {
    /// <summary>
    /// Marks the attributes with a helper interface that this namespace can have prioritization over other namespaces
    /// </summary>
    public interface INamespacePrioritizable {
        public int PrioritizeIndex { get; set; }
    }
}
