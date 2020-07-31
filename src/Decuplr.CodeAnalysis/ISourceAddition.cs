using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Decuplr.CodeAnalysis {
    public interface ISourceAddition {
        void AddSource(string fileName, string sourceCode);
        void AddSource(string fileName, SourceText text);
        void AddSource(GeneratedSourceText sourceText);
    }
}
