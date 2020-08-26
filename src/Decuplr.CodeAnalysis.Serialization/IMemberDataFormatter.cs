using System.Collections.Generic;
using Decuplr.CodeAnalysis.Diagnostics;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Serialization.Arguments;
using Decuplr.CodeAnalysis.Serialization.TypeComposite;
using Decuplr.CodeAnalysis.SourceBuilder;

namespace Decuplr.CodeAnalysis.Serialization {
    public interface IMemberDataFormatter : IFormatterParsingMethod<TypeSourceArgs> {
    }

    public interface IMemberComposingMethod {
        /// <summary>
        /// The relying member indexes in order for this method to correctly execute
        /// </summary>
        IReadOnlyList<int> DependentMemberIndex { get; }

        /// <summary>
        /// Get's the method body with the specificied <paramref name="methodId"/>
        /// </summary>
        /// <param name="methodId">A id that represents a specific chaining method</param>
        /// <param name="chained">The next method provider</param>
        /// <returns>The method body</returns>
        string GetMethodBody( string methodId, IChainMethodArgsProvider chained);
    }

    public interface IMemberComposingFeature : IGroupValidationProvider {
        bool ShouldFormat(MemberMetaInfo member);
        IMemberComposingMethod GetComposingMethods(MemberMetaInfo member, IComponentCollection components, IThrowCollection throwCollection);
    }
}
