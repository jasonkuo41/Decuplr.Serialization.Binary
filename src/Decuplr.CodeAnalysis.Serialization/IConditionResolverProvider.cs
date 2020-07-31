﻿using Decuplr.CodeAnalysis.Serialization.TypeComposite;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.Diagnostics;

namespace Decuplr.CodeAnalysis.Serialization {
    public interface IConditionResolverProvider : IGroupValidationProvider {
        bool ShouldFormat(MemberMetaInfo member);
        IConditionalFormatter GetResolver(MemberMetaInfo member, IThrowCollection throwCollection);
    }
}