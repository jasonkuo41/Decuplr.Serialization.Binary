using System;
using Decuplr.CodeAnalysis.Meta;
using Decuplr.Serialization.SourceBuilder;

namespace Decuplr.CodeAnalysis.Serialization.TypeComposite {
    public interface ITypeComposerBuilder {
        ITypeComposer BuildTypeComposer(SchemaLayout layout, IComponentProvider provider, GeneratingTypeName typeName, Func<MemberMetaInfo, GeneratingTypeName> memberCompositeNameFactory);
    }
}
