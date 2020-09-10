using System;
using System.Collections.Immutable;

namespace Decuplr.Sourceberg.Internal {
    internal struct AnalyzerGroupInfo<TTarget, TContext, TKind> where TTarget : class where TKind : Enum {
        
        public AnalyzerGroup<TTarget, TContext> Group { get; }
        public ImmutableArray<TKind> SelectedEnumKinds { get; }

        public AnalyzerGroupInfo(AnalyzerGroup<TTarget, TContext> group, ImmutableArray<TKind> selectedEnumKinds) {
            Group = group;
            SelectedEnumKinds = selectedEnumKinds;
        }

        public void Deconstruct(out AnalyzerGroup<TTarget, TContext> group, out ImmutableArray<TKind> selectedKinds) {
            group = Group;
            selectedKinds = SelectedEnumKinds;
        }

        public static implicit operator AnalyzerGroupInfo<TTarget, TContext, TKind>((AnalyzerGroup<TTarget, TContext> group, ImmutableArray<TKind> selectedKinds) tuple) 
            => new AnalyzerGroupInfo<TTarget, TContext, TKind>(tuple.group, tuple.selectedKinds);
    }

}
