using System;
using System.Collections.Generic;

namespace Decuplr.Sourceberg.Internal {
    internal abstract class AnalyzerGroupNode<TKind> where TKind : notnull {

        private AnalyzerGroupNode<TKind>? _nextNode;
        public AnalyzerGroup<TKind> Group { get; }
        
        public abstract Type AnalyzerType { get; }
        public Func<TKind, IEnumerable<TKind>>? PreviousTransisitionMultiple { get; }
        public Func<TKind, TKind>? PreviousTransisitionSingle { get; }

        protected AnalyzerGroupNode(AnalyzerGroup<TKind> parent) {
            Group = parent;
        }

        protected AnalyzerGroupNode(AnalyzerGroup<TKind> parent, Func<TKind, TKind>? transitionSingle) {
            Group = parent;
            PreviousTransisitionSingle = transitionSingle;
        }

        protected AnalyzerGroupNode(AnalyzerGroup<TKind> parent, Func<TKind, IEnumerable<TKind>>? transitionMultiple) {
            Group = parent;
            PreviousTransisitionMultiple = transitionMultiple;
        }

        public void SetNextNode(AnalyzerGroupNode<TKind> nextNode) {
            if (_nextNode is { })
                throw new InvalidOperationException("Adding more childs to a singular analyzer node is not allowed");
            _nextNode = nextNode;
            Group.Add(nextNode);
        }
    }

}
