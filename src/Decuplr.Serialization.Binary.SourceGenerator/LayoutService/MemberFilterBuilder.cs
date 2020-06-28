using System;
using System.Collections.Generic;
using System.Linq;
using Decuplr.Serialization.Binary.AnalysisService;
using Microsoft.CodeAnalysis;

namespace Decuplr.Serialization.Binary.LayoutService {
    internal class MemberFilterBuilder {

        private Func<MemberMetaInfo, bool>? _filters;

        public Func<MemberMetaInfo, bool> Filters => _filters ?? new Func<MemberMetaInfo, bool>(member => true);

        public MemberFilterBuilder SelectMember(Func<MemberMetaInfo, bool> filter) {
            _filters += filter;
            return this;
        }

        public MemberFilterBuilder SelectKind(params SymbolKind[] kinds) {
            var kindSet = new HashSet<SymbolKind>(kinds);
            _filters += member => kindSet.Contains(member.Symbol.Kind);
            return this;
        }

    }
}
