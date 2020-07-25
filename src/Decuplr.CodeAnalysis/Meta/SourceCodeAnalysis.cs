using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Decuplr.Serialization.AnalysisService;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.CodeAnalysis.Meta {

    // We currently don't support partial methods since it's fairly limited, and none of our feature would utilize it
    public class SourceCodeAnalysis {

        public IReadOnlyList<NamedTypeMetaInfo> ContainingTypes { get; }

        public SourceCodeAnalysis(ITypeSymbolProvider provider, ICompilationInfo compilationInfo, Compilation compilation, ICompilationLifetime lifetime, params SymbolKind[] memberKinds) {
            var ct = lifetime.OnCompilationCancelled;
            var types = new Dictionary<INamedTypeSymbol, List<SyntaxModelPair>>();

            foreach (var syntax in compilationInfo.DeclarationSyntaxes) {
                var model = compilation.GetSemanticModel(syntax.SyntaxTree, true);
                var typeSymbol = model.GetDeclaredSymbol(syntax, ct);
                if (typeSymbol is null)
                    continue;
                if (types.TryGetValue(typeSymbol, out var syntaxList))
                    syntaxList.Add((syntax, model));
                else
                    types.Add(typeSymbol, new List<SyntaxModelPair>() { (syntax, model) });
            }

            ContainingTypes = types.Select(type => new NamedTypeMetaInfo(provider, type.Key, type.Value, memberKinds, ct)).ToList();
        }

    }

}
