namespace Decuplr.Serialization.CodeGeneration {
    public interface IFunctionProvider<TArgs> {
        string GetFunctionBody(string nextFunc, TArgs args);
    }
}
