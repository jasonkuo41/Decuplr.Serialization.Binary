using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.Formatting;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    public interface IFormatterStrategy {
        bool TryVerify();
        GeneratedSourceCode[] CreateRequiredCode();
        FormatterSourceCode CreateFormatter();
    }
}
