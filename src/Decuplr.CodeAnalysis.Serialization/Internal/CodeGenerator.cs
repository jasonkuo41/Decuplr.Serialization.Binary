using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization.StartupServices;
using Decuplr.CodeAnalysis.Serialization.TypeComposite;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.CodeAnalysis.Serialization.Internal {
    internal class CodeGenerator : ICodeGenerator {

        internal struct SchemaSolution {
            public SchemaLayout Schema { get; set; }
            public IParsingSolution Solution { get; set; }
        }

        internal class SyntaxVerification {

            private readonly IGenerationStartupServices _startupServices;
            private readonly List<IGenerationStartup> _startups;
            private readonly ISourceMetaAnalysis _analysis;
            private readonly IDiagnosticReporter _diagnostic;
            private readonly Dictionary<IGenerationStartup, List<SchemaSolution>> _schemaInfos = new Dictionary<IGenerationStartup, List<SchemaSolution>>();

            public bool HasVerified { get; private set; } = false;

            public SyntaxVerification(IGenerationStartupServices startupServices, IEnumerable<IGenerationStartup> startups, ISourceMetaAnalysis analysis, IDiagnosticReporter diagnostic) {
                _startupServices = startupServices;
                _analysis = analysis;
                _diagnostic = diagnostic;
                _startups = startups.ToList();
            }

            internal void VerifySyntax(out IReadOnlyDictionary<IGenerationStartup, List<SchemaSolution>> schemas) {
                if (HasVerified) {
                    schemas = _schemaInfos;
                    return;
                }
                HasVerified = true;

                var schemaInfos = new Dictionary<IGenerationStartup, List<SchemaInfo>>();

                foreach (var startup in _startups) {
                    foreach (var type in _analysis.GetMetaInfo(DefaultMemberSelector)) {
                        if (!startup.TryGetSchemaInfo(type, out var schemaInfo))
                            continue;
                        if (schemaInfos.TryGetValue(startup, out var list))
                            list.Add(schemaInfo);
                        else
                            schemaInfos.Add(startup, new List<SchemaInfo> { schemaInfo });
                        "Make the SchemaLayout obsolete I think";
                    }
                }

                schemas = schemaInfos.ToDictionary(x => x.Key, x => {
                    var service = _startupServices.GetStartupScopeService(x.Key);
                    var syntaxVerify = service.GetRequiredService<ISourceValidation>();
                    var solutions = service.GetServices<IParsingSolution>();
                    return x.Value.Select(x => ValidateSchemas(syntaxVerify, solutions, x)).ToList();
                });
            }

            private SchemaSolution ValidateSchemas(ISourceValidation syntaxVerify, IEnumerable<IParsingSolution> solutions, SchemaInfo schema) {
                // Verify the order
                var type = schema.SourceTypeInfo;
                var orderSelector = schema.OrderSelector;
                syntaxVerify.ValidateExternal(type, orderSelector, _diagnostic);
                if (_diagnostic.ContainsError && orderSelector.ContinueDiagnosticAfterError) {
                    return default;
                }
                var selection = new TypeMetaSelection(schema.SourceTypeInfo, orderSelector.GetOrder(type));
                var solution = ElectAndReportDiagnostic(syntaxVerify, solutions, selection);
                syntaxVerify.Validate(selection);
                return new SchemaSolution {
                    Schema = new SchemaLayout(schema, selection.SelectedMembers),
                    Solution = solution
                };

                IParsingSolution ElectAndReportDiagnostic(ISourceValidation syntaxVerify, IEnumerable<IParsingSolution> solutions, TypeMetaSelection selection) {
                    DiagnosticReporterCollection? lastReporter = null;
                    IParsingSolution? finalSolution = null;
                    foreach (var solution in solutions) {
                        finalSolution = solution;
                        lastReporter = new DiagnosticReporterCollection();
                        syntaxVerify.ValidateExternal(selection, solution, lastReporter);
                        if (!lastReporter.ContainsError)
                            break;
                    }
                    _diagnostic.ReportDiagnostic(lastReporter?.Diagnostics ?? throw NoSolutionIsFound());
                    return finalSolution ?? throw NoSolutionIsFound();
                }

                static Exception NoSolutionIsFound() => new InvalidOperationException("No Parsing Solution is not found");
            }
        }

        // We should make this configurable
        private static readonly HashSet<SymbolKind> DefaultMemberKinds = new HashSet<SymbolKind> { SymbolKind.Method, SymbolKind.Field, SymbolKind.Property, SymbolKind.Event };

        private readonly SyntaxVerification _syntaxVerify;
        private readonly DiagnosticReporter _diagnostics;
        private readonly ITypeParserDirector _director;
        private readonly IGenerationStartupServices _genservices;
        private readonly TypeSymbolCollection _symbolCollection;
        private readonly TypeParserHost _parserHost;
        private readonly IEnumerable<IGenerationFinalization> _finalizations;
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

        public CodeGenerator(ITypeParserDirector director,
                             IGenerationStartupServices genservices,
                             SyntaxVerification verification,
                             DiagnosticReporter diagnostics,
                             TypeSymbolCollection symbolCollection,
                             TypeParserHost parserHost,
                             IEnumerable<IGenerationFinalization> finalizations) {
            _diagnostics = diagnostics;
            _director = director;
            _syntaxVerify = verification;
            _genservices = genservices;
            _symbolCollection = symbolCollection;
            _parserHost = parserHost;
            _finalizations = finalizations;
        }

        private static bool DefaultMemberSelector(ISymbol memberSymbol) => DefaultMemberKinds.Contains(memberSymbol.Kind);

        public void VerifySyntax() => _syntaxVerify.VerifySyntax(out _);

        public void GenerateFiles() {
            _syntaxVerify.VerifySyntax(out var startupSchemas);

            // Don't generate any file if we have error in our diagnostics
            if (_diagnostics.ContainsError)
                return;

            if (_isGenerated)
                return;
            _isGenerated = true;

            _director.ComposeParser(startupSchemas.SelectMany(x => {
                var (startup, schemas) = x;
                var services = _genservices.GetStartupScopeService(startup);
                return schemas.Select(x => _parserHost.CreateBuilder(x.Schema, x.Solution));
            }).ToList());

            foreach (var finalizer in _finalizations)
                finalizer.OnGenerationFinalized(_parserHost.GeneratedParsers);
        }

    }
}
