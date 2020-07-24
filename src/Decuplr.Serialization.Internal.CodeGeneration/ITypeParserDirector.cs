using System.Threading;

namespace Decuplr.Serialization.CodeGeneration {
    public interface ITypeParserDirector {
        void ComposeParser(CancellationToken ct);
    }
}
