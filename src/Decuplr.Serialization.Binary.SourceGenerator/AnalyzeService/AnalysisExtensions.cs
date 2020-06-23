using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary.AnalyzeService {
    internal static class AnalysisExtensions {
        public static AttributeListing GetAttributes(this MemberDeclarationSyntax syntax, ISymbol sourceSymbol) {
            var typeAttributes = sourceSymbol.GetAttributes();
            var attributeList = syntax.AttributeLists.Select(attributeList =>
                attributeList.Attributes.Select(attribute => {
                    return (
                        Data: typeAttributes.Where(x => attribute.GetReference().Span.Equals(x.ApplicationSyntaxReference?.Span)).First(),
                        Location: attribute.GetLocation()
                    );
                })
            );

            return new AttributeListing {
                Lists = attributeList.Select(x => (IReadOnlyList<AttributeData>)x.ToList()).ToList(),
                Locations = attributeList.SelectMany(x => x).ToDictionary(x => x.Data, x => x.Location)
            };
        }
    }

}
