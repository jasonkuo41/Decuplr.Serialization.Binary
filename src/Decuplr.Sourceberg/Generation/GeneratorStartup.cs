using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;

namespace Decuplr.Sourceberg.Generation {
    public abstract class GeneratorStartup {
        public abstract bool ShouldCapture(SyntaxNode syntax);
        public abstract void ConfigureAnalyzer(AnalyzerSetupContext setup);
        public abstract void ConfigureServices(IServiceCollection services);
    }
}
