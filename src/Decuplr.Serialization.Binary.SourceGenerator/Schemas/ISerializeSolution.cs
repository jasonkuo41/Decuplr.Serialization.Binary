using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.Serialization.Binary.SourceGenerator.Schemas {

    interface ISerializeSolution {
        GeneratedSourceCode[] GetAdditionalFiles();
        GeneratedFormatFunction GetTrySerializeFunction();
        GeneratedFormatFunction GetSerializeFunction();
        GeneratedFormatFunction GetBinaryLengthFunction();
    }

}
