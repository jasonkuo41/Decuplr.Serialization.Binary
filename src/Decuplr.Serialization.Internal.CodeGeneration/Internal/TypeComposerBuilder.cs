using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.Serialization.LayoutService;

namespace Decuplr.Serialization.CodeGeneration.Internal {

    class TypeComposerPrecusor {

        public TypeLayout Type { get; }

        public IReadOnlyList<MemberComposerPrecusor> MemberComposers { get; }
    }

    class TypeComposerBuilder {

        private readonly TypeLayout _type;

        public TypeComposerBuilder(TypeLayout layout) {
            _type = layout;
        }

        public TypeComposerPrecusor Build(IDependencySourceProvider provider) {

        }
    }
}
