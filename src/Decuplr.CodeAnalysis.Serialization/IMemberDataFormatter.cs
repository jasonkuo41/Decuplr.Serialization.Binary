using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization.Arguments;
using Decuplr.CodeAnalysis.Serialization.TypeComposite;

namespace Decuplr.CodeAnalysis.Serialization {
    public interface IMemberDataFormatter : IFormatterParsingMethod<TypeSourceArgs> {
    }

    public interface IMemberComposingMethod {
        string GetMethodBody(string methodId, IChainMethodArgsProvider provider);
    }


    public interface IMemberComposingFeature : IGroupValidationProvider {
        bool ShouldFormat(MemberMetaInfo member);
        IMemberComposingMethod GetComposingMethods(MemberMetaInfo member, IComponentCollection provider, IThrowCollection throwCollection);
    }
}
