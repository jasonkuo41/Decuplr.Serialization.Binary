namespace Decuplr.Serialization.Binary.Schemas {
    interface IDeserializeSolution {
        GeneratedSourceCode[] GetAdditionalFiles();
        GeneratedFormatFunction GetDeserializeFunction();
    }
}
