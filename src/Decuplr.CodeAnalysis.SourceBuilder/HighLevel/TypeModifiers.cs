using System;

#if HIGH_LEVEL

namespace Decuplr.Serialization.SourceBuilder {
    [Flags]
    public enum TypeModifiers {
        None = 0,

        // Exclusive for classes
        Abstract = 0b001,
        Static = 0b_010,

        // Mutual for classes and structs
        Partial = 0b1_000,

        // Exclusive for structs
        ReadOnly = 0b1_000_000
    }
}

#endif