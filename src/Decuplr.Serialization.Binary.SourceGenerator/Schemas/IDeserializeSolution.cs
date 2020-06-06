namespace Decuplr.Serialization.Binary.SourceGenerator.Schemas {
    interface IDeserializeSolution {
        GeneratedSourceCode[] GetAdditionalFiles();
        GeneratedFormatFunction GetDeserializeFunction();
    }
}
