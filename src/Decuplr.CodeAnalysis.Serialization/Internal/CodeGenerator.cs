using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization.StartupServices;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.CodeAnalysis.Serialization.Internal {
    internal class CodeGenerator : ICodeGenerator {

        internal class SyntaxVerification {

            private readonly IGenerationStartupServices _startupServices;
            private readonly List<IGenerationStartup> _startups;
            private readonly ISourceMetaAnalysis _analysis;
            private readonly Dictionary<IGenerationStartup, List<SchemaInfo>> _schemaInfos = new Dictionary<IGenerationStartup, List<SchemaInfo>>();

            public bool HasVerified { get; private set; } = false;

            public SyntaxVerification(IGenerationStartupServices startupServices, IEnumerable<IGenerationStartup> startups, ISourceMetaAnalysis analysis) {
                _startupServices = startupServices;
                _analysis = analysis;
                _startups = startups.ToList();
            }

            internal void VerifySyntax(out IReadOnlyDictionary<IGenerationStartup, List<SchemaInfo>> schemas) {
                if (HasVerified) {
                    schemas = _schemaInfos;
                    return;
                }
                HasVerified = true;

                var schemaInfos = new Dictionary<IGenerationStartup, List<SchemaInfo>>();
                schemas = schemaInfos;

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

                foreach (var (startup, schemaList) in schemaInfos) {
                    var service = _startupServices.GetStartupScopeService(startup);
                    var syntaxVerify = service.GetRequiredService<ISourceValidation>();
                    foreach (var schema in schemaList) {
                        syntaxVerify.Validate(schema.SourceTypeInfo, schema.OrderSelector.IsCandidateMember);
                    }
                }
            }

        }

        // We should make this configurable
        private static readonly HashSet<SymbolKind> DefaultMemberKinds = new HashSet<SymbolKind> { SymbolKind.Method, SymbolKind.Field, SymbolKind.Property, SymbolKind.Event };

        private readonly SyntaxVerification _syntaxVerify;
        private readonly DiagnosticReporter _diagnostics;
        private readonly ITypeParserDirector _director;
        private readonly TypeSymbolCollection _symbolCollection;
        private bool _isGenerated = false;

        public event EventHandler<Diagnostic> OnReportedDiagnostic {
            add {
                if (_syntaxVerify.HasVerified)
                    throw new InvalidOperationException("Cannot listen to new diagnostic reports after syntax is verified");
                _diagnostics.OnReportedDiagnostic += value;
            }
            remove => _diagnostics.OnReportedDiagnostic -= value;
        }

        public event EventHandler<GeneratedSourceText> OnGeneratedSource {
            add {
                if (_isGenerated)
                    throw new InvalidOperationException("Cannot listen to new generated source text after source is generated");
                _symbolCollection.OnSourceGenerated += value;
            }
            remove => _symbolCollection.OnSourceGenerated -= value;
        }

        public CodeGenerator(ITypeParserDirector director, SyntaxVerification verification, DiagnosticReporter diagnostics, TypeSymbolCollection symbolCollection) {
            _diagnostics = diagnostics;
            _director = director;
            _syntaxVerify = verification;
            _symbolCollection = symbolCollection;
        }

        private static bool DefaultMemberSelector(ISymbol memberSymbol) => DefaultMemberKinds.Contains(memberSymbol.Kind);

        public void VerifySyntax() => _syntaxVerify.VerifySyntax(out _);

        public void GenerateFiles() {
            _syntaxVerify.VerifySyntax(out var schemas);

            // Don't generate any file if we have error in our diagnostics
            if (_diagnostics.ContainsError)
                return;

            if (_isGenerated)
                return;
            _isGenerated = true;

            var layouts = schemas.SelectMany(x => x.Value).Select(x => new SchemaLayout(x, x.OrderSelector.GetOrder(x.SourceTypeInfo)));
            _director.ComposeParser();
        }

    }
}
