using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.CodeGeneration {
    public interface ISourceAddition {
        void AddSource(GeneratedSourceCode sourceCode);
    }
}
