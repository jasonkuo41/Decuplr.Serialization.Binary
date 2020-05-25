using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary.SourceGenerator {
    interface IDeserializeSolution {
        GeneratedSourceCode[] GetAdditionalFiles();
        FormattingFunction GetDeserializeFunction();
    }

    interface ISerializeSolution {
        GeneratedSourceCode[] GetAdditionalFiles();
        FormattingFunction GetSerializeFunction();
    }
}
