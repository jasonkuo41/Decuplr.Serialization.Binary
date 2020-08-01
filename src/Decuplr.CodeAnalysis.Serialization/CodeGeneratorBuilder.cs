using System;
using System.Collections.Generic;
using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization.Internal;
using Decuplr.CodeAnalysis.Serialization.StartupServices;
using Decuplr.CodeAnalysis.Serialization.TypeComposite;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.CodeAnalysis.Serialization {

    public class CodeGeneratorBuilder {

        private class CodeGeneratorFactory : ICodeGeneratorFactory {
            private readonly IServiceProvider _services;

            public CodeGeneratorFactory(IServiceProvider services) {
                _services = services;
            }

            public void RunGeneration(Compilation compilation, IEnumerable<TypeDeclarationSyntax> syntaxes, Action<ICodeGenerator> generatorAction) {
                using var scope = _services.CreateScope();
                var service = scope.ServiceProvider;
                {
                    var compileInfo = service.GetRequiredService<CompilationInfo>();
                    compileInfo.SourceCompilation = compilation;
                    compileInfo.DeclarationSyntaxes = syntaxes;
                }
                generatorAction(service.GetRequiredService<ICodeGenerator>());
            }
        }

        private readonly HashSet<Type> _startups = new HashSet<Type>();

        private Action<IServiceCollection>? _serviceConfig;

        public CodeGeneratorBuilder AddStartup<TProvider>() where TProvider : class, IGenerationStartup {
            if (!_startups.Add(typeof(IGenerationStartup)))
                return this;
            ConfigureStartupServices(services => services.AddSingleton<IGenerationStartup, TProvider>());
            return this;
        }

        public CodeGeneratorBuilder ConfigureStartupServices(Action<IServiceCollection> serviceConfiguration) {
            _serviceConfig += serviceConfiguration;
            return this;
        }

        public ICodeGeneratorFactory BuildGenerator<TProvider>(Func<IServiceProvider, TProvider> providerFactory) where TProvider : class, ITypeParserDirector {
            if (_startups.Count == 0)
                throw new ArgumentException("No entry startup is provided. Code Generation Failed");

            var services = new ServiceCollection();

            // Singleton Area (Compilation Agnostic Services)
            {
                services.AddSingleton<IUniqueNameProvider>(new UniqueNameProvider());
                services.AddSingleton<ICodeGeneratorFactory, CodeGeneratorFactory>();
            }

            services.AddScoped<ICodeGenerator, CodeGenerator>();
            services.AddScoped<ITypeParserDirector>(providerFactory);

            services.AddScoped<CompilationInfo>();
            services.AddScoped<ICompilationInfo>(services => services.GetRequiredService<CompilationInfo>());

            services.AddScoped<DiagnosticReporter>();
            services.AddScoped<IDiagnosticReporter>(services => services.GetRequiredService<DiagnosticReporter>());
            // Add TypeSymbolProvider / SourceAddition
            {   
                services.AddScoped<TypeSymbolCollection>();
                services.AddScoped<ITypeSymbolProvider>(provider => provider.GetRequiredService<TypeSymbolCollection>());
                services.AddScoped<ISourceAddition>(provider => provider.GetRequiredService<TypeSymbolCollection>());
            }
            services.AddSourceMetaAnalysis(); // ISourceMetaAnalysis
            services.AddGenerationStartupServices();

            // Setup some internal services
            services.AddScoped<CodeGenerator.SyntaxVerification>();

            _serviceConfig?.Invoke(services);

            return services.BuildServiceProvider().GetRequiredService<ICodeGeneratorFactory>();
        }

    }
}
