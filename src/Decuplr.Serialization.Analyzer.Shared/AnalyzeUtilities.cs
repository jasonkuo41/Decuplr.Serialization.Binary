using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Decuplr.Serialization.Binary.Analyzers
{
    internal static class AnalyzeUtilities {
        public static IReadOnlyList<IReadOnlyList<AnalyzedAttribute>> GetAttributeDataFrom(this MemberDeclarationSyntax syntax, ISymbol sourceSymbol) {
            var typeAttributes = sourceSymbol.GetAttributes();
            return syntax.AttributeLists.Select(list => {
                return list.Attributes.Select(ho => {
                    return new AnalyzedAttribute {
                        // This get's the AttributeData from the type
                        // TODO : Please make sure this works!
                        Data = typeAttributes.Where(x => ho.GetReference().Equals(x.ApplicationSyntaxReference)).First(),
                        Location = ho.GetLocation()
                    };
                }).ToList();
            }).ToList();
        }

        public static T? GetNamedArgumentValue<T>(this AttributeData data, string propertyName) where T : struct {
            var value = data.NamedArguments.First(x => x.Key == propertyName).Value.Value;
            if (value is null)
                return default;
            return (T)value;
        }
    }
}
