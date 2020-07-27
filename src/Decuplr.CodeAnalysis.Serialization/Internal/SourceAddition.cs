using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Decuplr.CodeAnalysis.Serialization.Internal {
    internal class SourceAddition : ISourceAddition {

        private readonly Action<GeneratedSourceText>? _sourceTextCb;

        public SourceAddition(Action<GeneratedSourceText>? sourceTextCb) {
            _sourceTextCb = sourceTextCb;
        }

        public void AddSource(string fileName, string sourceCode) {
            if (_sourceTextCb is null)
                return;
            AddSource(fileName, SourceText.From(sourceCode, Encoding.UTF8));
        }

        public void AddSource(string fileName, SourceText text) => AddSource(new GeneratedSourceText(fileName, text));

        public void AddSource(GeneratedSourceText sourceText) => _sourceTextCb?.Invoke(sourceText);
    }
}
