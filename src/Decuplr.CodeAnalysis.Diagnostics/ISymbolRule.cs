namespace Decuplr.CodeAnalysis.Diagnostics {
    public interface ISymbolRule<TMeta> : ISymbolCondition<TMeta, ISymbolRule<TMeta>> { }
}
