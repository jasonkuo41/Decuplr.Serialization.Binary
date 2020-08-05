using Decuplr.CodeAnalysis.Serialization.TypeComposite;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization {
    public interface ITypeParserBuilder {

        SchemaLayout Layout { get; }

        ITypeParserBuilder MakeGenericType(params ITypeSymbol[] symbols);

        TypeParserInfo CreateParser(IComponentProvider provider, string uniqueName);
    }

}
