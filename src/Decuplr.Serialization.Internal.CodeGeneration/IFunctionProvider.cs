namespace Decuplr.Serialization.CodeGeneration {
    public interface IFunctionProvider<TArgs> {
        /// <summary>
        /// Get's the function body of the arguments, if there no chaining function available the nextFunc is null
        /// </summary>
        /// <param name="nextFunc"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        string GetFunctionBody(string? nextFunc, TArgs args);
    }
}
