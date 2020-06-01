using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary.SourceGenerator {

    interface ISerializeSolution {
        GeneratedSourceCode[] GetAdditionalFiles();
        GeneratedFormatFunction GetSerializeFunction();
    }

}
