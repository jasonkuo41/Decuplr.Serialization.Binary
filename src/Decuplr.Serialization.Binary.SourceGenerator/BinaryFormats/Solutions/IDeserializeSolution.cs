namespace Decuplr.Serialization.Binary.SourceGenerator {
    interface IDeserializeSolution {
        GeneratedSourceCode[] GetAdditionalFiles();
        GeneratedFormatFunction GetDeserializeFunction();
    }
}
