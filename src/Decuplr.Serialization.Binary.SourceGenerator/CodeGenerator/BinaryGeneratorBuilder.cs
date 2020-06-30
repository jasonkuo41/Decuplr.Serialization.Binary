using System;
using System.Collections.Generic;

namespace Decuplr.Serialization.Binary.CodeGenerator {
    internal class BinaryGeneratorBuilder {

        private readonly List<IGeneratorProvider> _providers = new List<IGeneratorProvider>();

        public void AddProvider<TProvider>() where TProvider : IGeneratorProvider, new() => _providers.Add(new TProvider());

        public BinaryGenerator CreateGenerator() {
            if (_providers.Count == 0)
                throw new InvalidOperationException("Provider must be provided to generate a binary generator");
            return new BinaryGenerator(_providers);
        }
    }
}
