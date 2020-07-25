using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.AnalysisService {
    internal struct SyntaxModelPair {
        public TypeDeclarationSyntax Syntax { get; set; }
        public SemanticModel Model { get; set; }

        public SyntaxModelPair((TypeDeclarationSyntax, SemanticModel) tuple) {
            Syntax = tuple.Item1;
            Model = tuple.Item2;
        }

        public static implicit operator SyntaxModelPair((TypeDeclarationSyntax, SemanticModel) tuple) => new SyntaxModelPair(tuple);
    }

}
