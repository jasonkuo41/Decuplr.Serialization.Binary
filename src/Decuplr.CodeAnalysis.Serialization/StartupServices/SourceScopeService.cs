using System;

namespace Decuplr.CodeAnalysis.Serialization.StartupServices {
    internal class SourceScopeService {
        private IServiceProvider? _provider;

        public IServiceProvider CurrentScopeService {
            get => _provider ?? throw new InvalidOperationException("Service Provier is not initialized");
            set {
                if (_provider != null)
                    throw new ArgumentException($"Cannot set a already initialized value", nameof(CurrentScopeService));
                _provider = value;
            }
        }
    }
}
