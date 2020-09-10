using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Decuplr.Sourceberg.Services {
    public interface ISourceAddition {
        SyntaxTree AddSource(string fileName, string sourceCode);
        SyntaxTree AddSource(string hintName, SourceText sourceText);
    }
}
