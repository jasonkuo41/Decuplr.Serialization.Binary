namespace Decuplr.Serialization.CodeGeneration {
    public interface IFunctionSource<TArgs> {
        string GetNextFunction(TArgs args);
    }
}
