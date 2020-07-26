using Decuplr.CodeAnalysis.Serialization.Arguments;
using Decuplr.Serialization.SourceBuilder;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite {
    /// <summary>
    /// Represents the info of each component, how to generate the initialization method and it's type name
    /// </summary>
    public interface IComponentTypeInfo : IComposerMethodBodyBuilder {
        /// <summary>
        /// The full name of the component, for example "TypeParser`T" or "ByteOrder"
        /// </summary>
        string FullTypeName { get; }

        void ProvideInitialize(CodeNodeBuilder builder, string discoveryName);

        void ProvideTryInitialize(CodeNodeBuilder builder, string discoveryName, OutArgs<bool> isSuccess);
    }
}
