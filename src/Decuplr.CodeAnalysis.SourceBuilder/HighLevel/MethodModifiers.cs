﻿using System;

#if HIGH_LEVEL

namespace Decuplr.Serialization.SourceBuilder {
    [Flags]
    public enum MethodModifiers {
        None = 0,
        Override = 0b01,
        Static = 0b10,
    }
}

#endif