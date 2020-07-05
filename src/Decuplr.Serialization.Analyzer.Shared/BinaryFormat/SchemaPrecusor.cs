﻿using System.Collections.Generic;

namespace Decuplr.Serialization.Analyzer.BinaryFormat {
    public struct SchemaPrecusor {
        public bool NeverDeserialize { get; set; }

        public bool IsSealed { get; set; }

        public LayoutOrder RequestLayout { get; set; }

        public IReadOnlyList<string> TargetNamespaces { get; set; }

        // public InheritIndexRule InheritRule { get; set; }
    }

}
