using System.Collections.Generic;
using System.ComponentModel;
using Decuplr.Sourceberg.Generation;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg.Internal {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class SourcebergSyntaxReceiver : ISyntaxReceiver {
        private readonly List<SyntaxNode> _nodes = new List<SyntaxNode>();

        internal IReadOnlyList<SyntaxNode> CapturedNodes => _nodes;
        internal GeneratorStartup Startup { get; }

        internal SourcebergSyntaxReceiver(GeneratorStartup startup) => Startup = startup;

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
            if (Startup.ShouldCapture(syntaxNode))
                _nodes.Add(syntaxNode);
        }
    }

}
