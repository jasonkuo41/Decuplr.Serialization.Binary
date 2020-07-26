namespace Decuplr.CodeAnalysis.Serialization.TypeComposite {
    public interface IChainedMethodBodyProvider<TArgs> {
        /// <summary>
        /// Get's the function body of the arguments, if there no chaining function available the nextFunc is null
        /// </summary>
        /// <param name="nextMethodName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        string GetMethodBody(string? nextMethodName, TArgs args);
    }
}
