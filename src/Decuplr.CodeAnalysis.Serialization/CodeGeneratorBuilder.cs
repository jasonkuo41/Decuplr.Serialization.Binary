using System;
using System.Collections.Generic;
using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization.Internal;
using Decuplr.CodeAnalysis.Serialization.TypeComposite;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.CodeAnalysis.Serialization {

    public class CodeGeneratorBuilder {

        private readonly Compilation _compilation;
        private readonly IEnumerable<TypeDeclarationSyntax> _syntaxes;
        private readonly ServiceCollection _services = new ServiceCollection();
        private readonly HashSet<Type> _startups = new HashSet<Type>();

        private Action<IServiceCollection>? _serviceConfig;
        private Action<Diagnostic>? _diagnosticCb;
        private Action<GeneratedSourceText>? _sourceCallback;

        public CodeGeneratorBuilder(Compilation compilation, IEnumerable<TypeDeclarationSyntax> syntaxes) {
            _compilation = compilation;
            _syntaxes = syntaxes;
        }

        public CodeGeneratorBuilder AddStartup<TProvider>() where TProvider : class, IGenerationStartup {
            if (!_startups.Add(typeof(IGenerationStartup)))
                return this;
            _services.AddSingleton<IGenerationStartup, TProvider>();
            return this;
        }

        public CodeGeneratorBuilder ConfigureStartupServices(Action<IServiceCollection> serviceConfiguration) {
            _serviceConfig += serviceConfiguration;
            return this;
        }

        public CodeGeneratorBuilder OnReportedDiagnostics(Action<Diagnostic> diagnosticCb) {
            _diagnosticCb += diagnosticCb;
            return this;
        }

        public CodeGeneratorBuilder OnAddSource(Action<GeneratedSourceText> sourceCallback) {
            _sourceCallback = sourceCallback;
            return this;
        }

        public ICodeGenerator CreateGenerator<TProvider>(Func<IServiceProvider, TProvider> providerFactory) where TProvider : class, ITypeParserDirector {
            if (_startups.Count == 0)
                throw new ArgumentException("No entry startup is provided. Code Generation Failed");

            _services.AddSingleton<ICodeGenerator, CodeGenerator>();
            _services.AddSingleton<IStartupServiceProvider, StartupServiceProvider>(services => new StartupServiceProvider(services.GetServices<IGenerationStartup>(), _services, services));
            _services.AddSingleton<ITypeParserDirector>(providerFactory);
            _services.AddSingleton<ICompilationInfo>(new CompilationInfo(_compilation, _syntaxes));
            _services.AddSingleton<IDiagnosticReporter>(new DiagnosticReporter(_diagnosticCb));
            _services.AddSingleton<ISourceAddition>(new SourceAddition(_sourceCallback));
            _services.AddSourceMetaAnalysis(); // ISourceMetaAnalysis
            _services.AddSourceValidation(); // ISourceValidation

            _serviceConfig?.Invoke(_services);

            var appServices = _services.BuildServiceProvider();
            return appServices.GetRequiredService<ICodeGenerator>();
        }
    }
}
