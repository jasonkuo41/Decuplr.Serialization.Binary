using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.SourceBuilder {
    public static class CodeBuilderExtensions {
        private static void AddPartialClassNode(this CodeNodeBuilder node, INamedTypeSymbol symbol, Action<CodeNodeBuilder> nodeAction) {
            node.AddNode(symbol.DeclaredAccessibility, $"{(symbol.IsStatic ? "static" : null)} partial {symbol.TypeKind.ToString().ToLower()} {symbol.ToDisplayString(NullableFlowState.None, SymbolDisplayFormat.MinimallyQualifiedFormat)}", nodeAction);
        }

        public static void AddPartialClass(this SourceCodeFileBuilder builder, INamedTypeSymbol symbol, Action<CodeNodeBuilder> nodeBuilder) {
            Action<CodeNodeBuilder>? previousNode = null;
            var nestedTypes = symbol.GetContainingTypes().ToList();

            for (var i = nestedTypes.Count - 1; i > 0; --i) {
                var containSymbol = nestedTypes[i];
                // Force capture value, so we don't StackOverflow
                var rnode = previousNode;
                previousNode = node => node.AddPartialClassNode(containSymbol, rnode ?? nodeBuilder);
            }
            builder.AddPartialClassNode(nestedTypes[0], previousNode ?? nodeBuilder);
        }

    }
}
