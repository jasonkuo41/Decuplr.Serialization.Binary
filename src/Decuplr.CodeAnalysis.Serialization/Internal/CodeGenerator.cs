using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.CodeAnalysis.Serialization.Internal {
    internal class CodeGenerator : ICodeGenerator {

        internal class SyntaxVerification {

            private readonly StartupServiceProvider _startupServices;
            private readonly ICompilationInfo _compilationInfo;
            private readonly List<IGenerationStartup> _startups;
            private readonly ISourceMetaAnalysis _analysis;

            private bool _syntaxVerified = false;

            public SyntaxVerification(StartupServiceProvider startupServices, ICompilationInfo compilationInfo, IEnumerable<IGenerationStartup> startups, ISourceMetaAnalysis analysis) {
                _startupServices = startupServices;
                _compilationInfo = compilationInfo;
                _analysis = analysis;
                _startups = startups.ToList();
            }

            public void VerifySyntax() {
                if (_syntaxVerified)
                    return;
                _syntaxVerified = true;

                var schemaInfos = new Dictionary<IGenerationStartup, List<SchemaInfo>>();
                foreach (var startup in _startups) {
                    foreach (var type in _analysis.GetMetaInfo(DefaultMemberSelector)) {
                        if (!startup.TryGetSchemaInfo(type, out var schemaInfo))
                            continue;
                        if (schemaInfos.TryGetValue(startup, out var list))
                            list.Add(schemaInfo);
                        else
                            schemaInfos.Add(startup, new List<SchemaInfo> { schemaInfo });
                    }
                }

                foreach (var (startup, schemas) in schemaInfos) {
                    var service = _startupServices[startup];
                    var syntaxVerify = service.GetRequiredService<ISourceValidation>();
                    foreach (var schema in schemas) {
                        syntaxVerify.Validate(schema.SourceTypeInfo, schema.OrderSelector.IsValidMember);
                    }
                }
            }

        }

        // We should make this configurable
        private static readonly HashSet<SymbolKind> DefaultMemberKinds = new HashSet<SymbolKind> { SymbolKind.Method, SymbolKind.Field, SymbolKind.Property, SymbolKind.Event };

        private readonly SyntaxVerification _syntaxVerify;
        private readonly IDiagnosticReporter _diagnostics;
        private readonly ITypeParserDirector _director;

        public CodeGenerator(ITypeParserDirector director, SyntaxVerification verification, IDiagnosticReporter diagnostics) {
            _diagnostics = diagnostics;
            _director = director;
            _syntaxVerify = verification;
        }

        private static bool DefaultMemberSelector(ISymbol memberSymbol) => DefaultMemberKinds.Contains(memberSymbol.Kind);

        public void VerifySyntax() => _syntaxVerify.VerifySyntax();

        public void GenerateFiles() {
            VerifySyntax();

            // Don't generate any file if we have error in our diagnostics
            if (_diagnostics.ContainsError)
                return;

            _director.ComposeParser();
        }

    }
}
