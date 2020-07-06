namespace Decuplr.Serialization.CodeGeneration {
    public interface IFunctionProvider<TArgs> {
        string GetFunctionBody(IFunctionSource<TArgs> nextFunc, TArgs args);
    }
}
