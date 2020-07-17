using Decuplr.Serialization.CodeGeneration.Arguments;
using Decuplr.Serialization.CodeGeneration.ParserGroup;
using Decuplr.Serialization.SourceBuilder;

namespace Decuplr.Serialization.CodeGeneration {
    public interface IComponentData : IParsingMethodBody {
        /// <summary>
        /// The full name of the component, for example "TypeParser`T" or "ByteOrder"
        /// </summary>
        string FullName { get; }

        void ProvideInitialize(CodeNodeBuilder builder, string discoveryName);

        void ProvideTryInitialize(CodeNodeBuilder builder, string discoveryName, OutArgs<bool> isSuccess);
    }
}
