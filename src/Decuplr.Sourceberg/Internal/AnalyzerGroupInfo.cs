using System;
using System.Collections.Immutable;

namespace Decuplr.Sourceberg.Internal {
    internal struct AnalyzerGroupInfo<TTarget, TKind> where TTarget : class where TKind : Enum {
        
        public AnalyzerGroup<TTarget> Group { get; }
        public ImmutableArray<TKind> SelectedEnumKinds { get; }

        public AnalyzerGroupInfo(AnalyzerGroup<TTarget> group, ImmutableArray<TKind> selectedEnumKinds) {
            Group = group;
            SelectedEnumKinds = selectedEnumKinds;
        }

        public void Deconstruct(out AnalyzerGroup<TTarget> group, out ImmutableArray<TKind> selectedKinds) {
            group = Group;
            selectedKinds = SelectedEnumKinds;
        }

        public static implicit operator AnalyzerGroupInfo<TTarget, TKind>((AnalyzerGroup<TTarget> group, ImmutableArray<TKind> selectedKinds) tuple) => new AnalyzerGroupInfo<TTarget, TKind>(tuple.group, tuple.selectedKinds);
    }

}
