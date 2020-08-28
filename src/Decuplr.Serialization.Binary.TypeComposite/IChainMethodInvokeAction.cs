using Decuplr.CodeAnalysis.SourceBuilder;

namespace Decuplr.Serialization.Binary.TypeComposite {
    public interface IChainMethodInvokeAction {
        string this[TypeName type] { get; set; }
        string this[TypeName type, int index] { get; set; }
    }
}
