using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Decuplr.Sourceberg.Services {
    public interface IAttributeLayoutProvider {
        IAttributeCollection GetAttributes(ISymbol symbol);
    }
}
