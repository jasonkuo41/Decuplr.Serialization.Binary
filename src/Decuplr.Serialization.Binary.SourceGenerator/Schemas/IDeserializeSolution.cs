namespace Decuplr.Serialization.Binary.Schemas {
    interface IDeserializeSolution {
        GeneratedSourceCode[] GetAdditionalFiles();
        string GetDeserializeFunction(string functionName);
    }
}
