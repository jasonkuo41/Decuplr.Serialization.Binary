using System;
using System.Collections.Generic;

namespace Decuplr.Serialization.LayoutService {
    public struct SchemaPrecusor {

        private IReadOnlyList<string>? _namespaces;
        
        public bool NeverDeserialize { get; set; }

        public bool IsSealed { get; set; }

        public LayoutOrder RequestLayout { get; set; }

        public IReadOnlyList<string> TargetNamespaces {
            get => _namespaces ?? Array.Empty<string>();
            set => _namespaces = value;
        }

        // public InheritIndexRule InheritRule { get; set; }

    }

}
