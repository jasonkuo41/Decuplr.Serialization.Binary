using System;
using System.Collections;
using System.Collections.Generic;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.CodeAnalysis.SourceBuilder;
using Microsoft.CodeAnalysis;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite {
    public interface ITypeComposerBuilder {
        ITypeComposer BuildTypeComposer(SchemaLayout layout, IComponentProvider provider, GeneratingTypeName typeName, Func<MemberMetaInfo, GeneratingTypeName> memberCompositeNameFactory);
    }

}
