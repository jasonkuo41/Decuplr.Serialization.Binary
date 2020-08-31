using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.CodeAnalysis;
using Decuplr.CodeAnalysis.Meta;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.TypeComposite {
    public interface IBinarySchema {

        /// <summary>
        /// The source type that is responsible for producing this schema info
        /// </summary>
        NamedTypeMetaInfo SourceType { get; }

        IReadOnlyList<MemberMetaInfo> SerializeOrder { get; }

        IReadOnlyList<MemberMetaInfo>? DeserializeOrder { get; }

        IReadOnlyList<string> OutputGroups { get; }

        IReadOnlyList<TypeName> ExternalArguments { get; }

        IReadOnlyList<TypeName> TargetTypes { get; }
    }
}
