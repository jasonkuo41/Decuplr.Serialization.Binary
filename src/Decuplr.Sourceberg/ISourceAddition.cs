using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Decuplr.Sourceberg {
    public interface ISourceAddition {
        void AddSource(string fileName, string sourceCode);
        void AddSource(GeneratedSourceText sourceText);
    }
}
