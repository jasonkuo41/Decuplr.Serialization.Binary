using System;
using Decuplr.CodeAnalysis.Meta;

namespace Decuplr.CodeAnalysis.Diagnostics {
    public interface ISourceValidation {
        void Validate(NamedTypeMetaInfo type, Func<MemberMetaInfo, bool> memberSelector);
    }
}