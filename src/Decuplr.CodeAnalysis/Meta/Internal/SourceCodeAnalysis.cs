using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Decuplr.CodeAnalysis.Meta.Internal {

    // We currently don't support partial methods since it's fairly limited, and none of our feature would utilize it
    internal class SourceCodeAnalysis : ISourceMetaAnalysis {

        private readonly ITypeSymbolProvider _provider;
        private readonly ICompilationInfo _compilationInfo;
        private readonly ICompilationLifetime _lifetime;

        private Dictionary<INamedTypeSymbol, List<SyntaxModelPair>>? _symbolLookup;

        public SourceCodeAnalysis(ITypeSymbolProvider provider, ICompilationInfo compilationInfo, ICompilationLifetime lifetime) {
            _provider = provider;
            _compilationInfo = compilationInfo;
            _lifetime = lifetime;
        }

        private Dictionary<INamedTypeSymbol, List<SyntaxModelPair>> GetSymbolLookup() {
            var types = new Dictionary<INamedTypeSymbol, List<SyntaxModelPair>>();
            var declaringSyntaxes = _compilationInfo.DeclarationSyntaxes;
            var compilation = _compilationInfo.SourceCompilation;
            var ct = _lifetime.OnCompilationCancelled;

            foreach (var syntax in declaringSyntaxes) {
                var model = compilation.GetSemanticModel(syntax.SyntaxTree, true);
                var typeSymbol = model.GetDeclaredSymbol(syntax, ct);
                if (typeSymbol is null)
                    continue;
                if (types.TryGetValue(typeSymbol, out var syntaxList))
                    syntaxList.Add((syntax, model));
                else
                    types.Add(typeSymbol, new List<SyntaxModelPair>() { (syntax, model) });
            }
            return types;
        }

        public IEnumerable<NamedTypeMetaInfo> GetMetaInfo() => GetMetaInfoCore(null);
        public IEnumerable<NamedTypeMetaInfo> GetMetaInfo(Func<ISymbol, bool> predicate) => GetMetaInfoCore(predicate);
        private IEnumerable<NamedTypeMetaInfo> GetMetaInfoCore(Func<ISymbol, bool>? predicate)
            => (_symbolLookup ??= GetSymbolLookup()).Select(type => new NamedTypeMetaInfo(_provider, type.Key, type.Value, predicate, _lifetime.OnCompilationCancelled));

    }

}
