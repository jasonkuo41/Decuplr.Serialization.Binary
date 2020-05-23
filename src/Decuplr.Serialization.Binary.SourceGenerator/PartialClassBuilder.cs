using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    class PartialClassBuilder {

        private static string DefaultSerializePoint(ITypeSymbol symbol) => $"___generated__no_invoke_{symbol.Name}";

        public static SourceText CreatePartialClass(ITypeSymbol symbol) {
            var builder = new CodeSnippetBuilder(symbol.ContainingNamespace.Name);
            builder.Using("System");
            builder.Using("System.ComponentModel");
            builder.Using("System.CodeDom.Compiler");

            builder.AddAttribute("[GeneratedCode]");
            builder.AddNode(symbol.DeclaredAccessibility, $"partial class {symbol.Name}.GeneratedParser ", node => {
                node.AddAttribute("[EditorBrowsable(EditorBrowsableState.Never)]");
                node.AddNode($"private {symbol.Name} (ReadOnlySpan<byte> binary) {(symbol.TypeKind == TypeKind.Struct ? ":this()" : null)}", node => {
                    node.AddStatement("");
                    // We need to throw exception when we see a property like this :
                    // public int Property => 123;
                });

                node.AddAttribute("[EditorBrowsable(EditorBrowsableState.Never)]");
                node.AddNode($"internal static bool {DefaultSerializePoint(symbol)} (ReadOnlySpan<byte> binary)", node => { 
                    
                });
            });
        }
    }

    class CodeNodeBuilder {

        public void AddNode(Accessibility accessibility, string nodename, Action<CodeNodeBuilder> builder) {

        }

        public void AddNode(string nodename, Action<CodeNodeBuilder> builder) {

        }

    }

    class CodeSnippetBuilder : CodeNodeBuilder {

        private readonly string ThisNamespace;
        private readonly List<string> Namespaces;

        public CodeSnippetBuilder(string thisNamespace) {

        }

        public void Using(string namespaceName) {

        }
    }
}
