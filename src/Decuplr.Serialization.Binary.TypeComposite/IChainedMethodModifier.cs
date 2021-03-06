﻿using Decuplr.CodeAnalysis;

namespace Decuplr.Serialization.Binary.TypeComposite {
    public interface IChainedMethodsModifier {
        string this[TypeName type] { get; set; }
        string this[TypeName type, int index] { get; set; }
    }
}
