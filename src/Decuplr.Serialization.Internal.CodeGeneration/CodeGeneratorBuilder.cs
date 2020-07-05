using System;
using System.Collections.Generic;

namespace Decuplr.Serialization.CodeGeneration {
    public class CodeGeneratorBuilder {

        private readonly List<IGeneratorProvider> _providers = new List<IGeneratorProvider>();

        public CodeGeneratorBuilder AddProvider<TProvider>() where TProvider : IGeneratorProvider, new() {
            _providers.Add(new TProvider());
            return this;
        }

        public ICodeGenerator CreateGenerator() {
            if (_providers.Count == 0)
                throw new InvalidOperationException("Provider must be provided to generate a binary generator");
            return new CodeGenerator(_providers);
        }
    }
}
