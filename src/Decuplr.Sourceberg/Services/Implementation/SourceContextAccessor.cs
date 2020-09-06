using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg.Services.Implementation {
    internal class SourceContextAccessor : IAnalysisLifetime {
        public CancellationToken OnOperationCanceled { get; set; }

        public Compilation? SourceCompilation { get; set; }
    }

    internal class TypeSymbolProvider : ITypeSymbolProvider, ISourceAddition {

        private class SymbolCollection : ITypeSymbolCollection {
            public Compilation DeclaringCompilation => throw new NotImplementedException();

            public INamedTypeSymbol? GetSymbol<T>() {
                throw new NotImplementedException();
            }

            public INamedTypeSymbol? GetSymbol(Type type) {
                throw new NotImplementedException();
            }

            public INamedTypeSymbol? GetSymbol(string metadataQualifyName) {
                throw new NotImplementedException();
            }
        }

        public ITypeSymbolCollection Current => throw new NotImplementedException();

        public ITypeSymbolCollection Source => throw new NotImplementedException();

        public INamedTypeSymbol GetSourceSymbol(INamedTypeSymbol symbol) {
            throw new NotImplementedException();
        }

        public void AddSource(string fileName, string sourceCode) {
            throw new NotImplementedException();
        }

        public void AddSource(GeneratedSourceText sourceText) {
            throw new NotImplementedException();
        }
    }
}
