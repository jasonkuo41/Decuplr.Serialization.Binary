using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.CodeAnalysis.Serialization {
    public interface ISourceAddition {
        void AddSource(string fileName, string sourceCode);
    }
}
