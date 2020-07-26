using System;
using System.Collections.Generic;
using Decuplr.CodeAnalysis.Diagnostics;
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

        public ICodeGenerator CreateGenerator<TProvider>(Func<IServiceProvider, TProvider> providerFactory) where TProvider : class, ITypeParserDirector {
            if (_startups.Count == 0)
                throw new ArgumentException("No entry startup is provided. Code Generation Failed");

            _services.AddSingleton<ITypeParserDirector>(providerFactory);
            _services.AddSingleton<ICompilationInfo>(new CompilationInfo(_compilation, _syntaxes));
            _services.AddSingleton<IDiagnosticReporter>(new DiagnosticReporter());

            if (_sourceAddition != null)
                _services.AddSingleton<ISourceAddition>(new SourceAddition(_sourceAddition));

            _serviceConfig?.Invoke(_services);

            return new CodeGenerator(_services);
        }
    }

}
